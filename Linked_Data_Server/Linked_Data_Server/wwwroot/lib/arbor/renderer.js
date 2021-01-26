function ajusteDeTexto(ctx, texto, x, y, maxWidth, alturaDeLinea) {
    // crea el array de las palabras del texto
    var palabrasRy = texto.split(" ");
    // inicia la variable var lineaDeTexto
    var lineaDeTexto = "";
    // un bucle for recorre todas las palabras
    for (var i = 0; i < palabrasRy.length; i++) {
        var testTexto = lineaDeTexto + palabrasRy[i] + " ";
        // calcula la anchura del texto textWidth
        var textWidth = ctx.measureText(testTexto).width;
        // si textWidth > maxWidth
        if (textWidth > maxWidth && i > 0) {
            // escribe en el canvas la lineaDeTexto
            ctx.fillText(lineaDeTexto, x, y);
            // inicia otra lineaDeTexto         
            lineaDeTexto = palabrasRy[i] + " ";
            // incrementa el valor de la variable y
            //donde empieza la nueva lineaDeTexto
            y += alturaDeLinea;
        } else {// de lo contrario, si textWidth <= maxWidth
            lineaDeTexto = testTexto;
        }
    }// acaba el bucle for
    // escribe en el canvas la �ltima lineaDeTexto
    ctx.fillText(lineaDeTexto, x, y);
}




(function () {

    Renderer = function (canvas) {
        var canvas = $(canvas).get(0)
        var ctx = canvas.getContext("2d");
        var gfx = arbor.Graphics(canvas)
        var particleSystem = null

        var that = {
            init: function (system) {
                particleSystem = system
                particleSystem.screenSize(canvas.width, canvas.height)
                particleSystem.screenPadding(40)

                that.initMouseHandling()

            },

            redraw: function () {

                if (!particleSystem) return

                gfx.clear() // convenience ƒ: clears the whole canvas rect

                // draw the nodes & save their bounds for edge drawing
                var nodeBoxes = {}
                particleSystem.eachNode(function (node, pt) {


                    // node: {mass:#, p:{x,y}, name:"", data:{}}
                    // pt:   {x:#, y:#}  node position in screen coords

                    // Load extra info
                    var image = node.data.image
                    var imageH = node.data.image_h
                    var imageW = node.data.image_w
                    var radius = 15

                    // determine the box size and round off the coords if we'll be 
                    // drawing a text label (awful alignment jitter otherwise...)
                    var label = node.data.label
                    var w = 20;
                    if (!("" + label).match(/^[ \t]*$/)) {
                        pt.x = Math.floor(pt.x)
                        pt.y = Math.floor(pt.y)
                    } else {
                        label = null
                    }

                    // draw a rectangle centered at pt
                    if (node.data.color) ctx.fillStyle = node.data.color
                    else ctx.fillStyle = "rgba(0,0,0,.2)"
                    if (node.data.color == 'none') ctx.fillStyle = "white"

                    // draw the text
                    if (label) {
                        ctx.font = "12px Helvetica"
                        ctx.textBaseline = 'top';
                        ctx.fillStyle = "black"
                        if (node.data.color == 'none') ctx.fillStyle = '#333333'
                        ajusteDeTexto(ctx, label, pt.x + (w/2) + 5, pt.y-(w/2), 100, 12);
                        //ctx.fillText(label || "", pt.x, pt.y + 4)
                        //ctx.fillText(label || "", pt.x, pt.y + 4)
                    }
                    if (node.data.image) {
                        // Custom image loading function
                        var pic = new Image()
                        pic.src = "iconos/" + node.data.image
                        ctx.drawImage(pic, pt.x - w / 2, pt.y - w / 2);
                        nodeBoxes[node.name] = [pt.x - w / 2, pt.y - w / 2, w, w]

                    } else {
                        if (node.data.color) ctx.fillStyle = node.data.color
                        else ctx.fillStyle = "rgba(0,0,0,.2)"
                        if (node.data.color == 'none') ctx.fillStyle = "grey"
                        gfx.oval(pt.x - w / 2, pt.y - w / 2, w, w, { fill: ctx.fillStyle })
                        nodeBoxes[node.name] = [pt.x - w / 2, pt.y - w / 2, w, w]
                    }
                })


                // draw the edges
                particleSystem.eachEdge(function (edge, pt1, pt2) {
                    // edge: {source:Node, target:Node, length:#, data:{}}
                    // pt1:  {x:#, y:#}  source position in screen coords
                    // pt2:  {x:#, y:#}  target position in screen coords

                    var weight = edge.data.weight
                    var color = edge.data.color

                    if (!color || ("" + color).match(/^[ \t]*$/)) color = null

                    // find the start point
                    var tail = intersect_line_box(pt1, pt2, nodeBoxes[edge.source.name])
                    var head = intersect_line_box(tail, pt2, nodeBoxes[edge.target.name])

                    ctx.save()
                    ctx.beginPath()
                    ctx.lineWidth = (!isNaN(weight)) ? parseFloat(weight) : 1
                    ctx.strokeStyle = (color) ? color : "#cccccc"
                    ctx.fillStyle = null

                    ctx.moveTo(tail.x, tail.y)
                    ctx.lineTo(head.x, head.y)
                    ctx.stroke()
                    ctx.restore()

                    // draw an arrowhead if this is a -> style edge
                    if (edge.data.directed) {
                        ctx.save()
                        // move to the head position of the edge we just drew
                        var wt = !isNaN(weight) ? parseFloat(weight) : 1
                        var arrowLength = 6 + wt
                        var arrowWidth = 2 + wt
                        ctx.fillStyle = (color) ? color : "#cccccc"
                        ctx.translate(head.x, head.y);
                        ctx.rotate(Math.atan2(head.y - tail.y, head.x - tail.x));

                        // delete some of the edge that's already there (so the point isn't hidden)
                        ctx.clearRect(-arrowLength / 2, -wt / 2, arrowLength / 2, wt)

                        // draw the chevron
                        ctx.beginPath();
                        ctx.moveTo(-arrowLength, arrowWidth);
                        ctx.lineTo(0, 0);
                        ctx.lineTo(-arrowLength, -arrowWidth);
                        ctx.lineTo(-arrowLength * 0.8, -0);
                        ctx.closePath();
                        ctx.fill();
                        ctx.restore()
                    }


                    //if (edge.data.name) {
                    //    ctx.strokeStyle = "rgba(0,0,0, .333)";
                    //    ctx.lineWidth = 1;
                    //    ctx.beginPath();
                    //    ctx.moveTo(pt1.x, pt1.y);
                    //    ctx.lineTo(pt2.x, pt2.y);
                    //    ctx.stroke();

                    //    ctx.fillStyle = "black";
                    //    ctx.font = 'italic 13px sans-serif';
                    //    ctx.fillText(edge.data.name, (pt1.x + pt2.x) / 2, (pt1.y + pt2.y) / 2);
                    //}


                })



            },
            initMouseHandling: function () {
                // no-nonsense drag and drop (thanks springy.js)
                selected = null;
                nearest = null;
                var dragged = null;
                var oldmass = 1
                var mouse_is_down = false;
                var mouse_is_moving = false
                var dom = $(canvas)

                // set up a handler object that will initially listen for mousedowns then
                // for moves and mouseups while dragging
                var handler = {
                    mousemove: function (e) {
                        if (!mouse_is_down) {
                            var pos = $(canvas).offset();
                            _mouseP = arbor.Point(e.pageX - pos.left, e.pageY - pos.top)
                            nearest = particleSystem.nearest(_mouseP);

                            if (!nearest.node) return false
                            selected = (nearest.distance < 50) ? nearest : null
                            if (selected && selected.node.data.link) {
                                dom.addClass('linkable')
                            } else {
                                dom.removeClass('linkable')
                            }
                        }
                        return false
                    },
                    clicked: function (e) {
                        var pos = $(canvas).offset();
                        _mouseP = arbor.Point(e.pageX - pos.left, e.pageY - pos.top)
                        nearest = particleSystem.nearest(_mouseP);

                        if (!nearest.node) return false
                        selected = (nearest.distance < 50) ? nearest : null

                        if (nearest && selected && nearest.node === selected.node) {
                            var link = selected.node.data.link
                            if (link.match(/^#/)) {
                                $(that).trigger({ type: "navigate", path: link.substr(1) })
                            } else {
                                window.open(link, "_blank")
                            }
                            return false
                        }
                    },
                    mousedown: function (e) {
                        var pos = $(canvas).offset();
                        _mouseP = arbor.Point(e.pageX - pos.left, e.pageY - pos.top)
                        selected = nearest = dragged = particleSystem.nearest(_mouseP);

                        if (dragged.node !== null) dragged.node.fixed = true

                        mouse_is_down = true
                        mouse_is_moving = false

                        $(canvas).bind('mousemove', handler.dragged)
                        $(window).bind('mouseup', handler.dropped)

                        return false
                    },
                    dragged: function (e) {
                        var old_nearest = nearest && nearest.node._id
                        var pos = $(canvas).offset();
                        var s = arbor.Point(e.pageX - pos.left, e.pageY - pos.top)

                        mouse_is_moving = true

                        if (!nearest) return
                        if (dragged !== null && dragged.node !== null) {
                            var p = particleSystem.fromScreen(s)
                            dragged.node.p = p
                        }

                        return false
                    },

                    dropped: function (e) {
                        if (dragged === null || dragged.node === undefined) return
                        if (dragged.node !== null) dragged.node.fixed = false
                        dragged.node.tempMass = 50
                        dragged = null
                        selected = null
                        $(canvas).unbind('mousemove', handler.dragged)
                        $(window).unbind('mouseup', handler.dropped)
                        _mouseP = null

                        if (mouse_is_moving) {
                            // console.log("was_dragged")
                        } else {
                            handler.clicked(e)
                        }

                        mouse_is_down = false

                        return false
                    }
                }
                $(canvas).mousedown(handler.mousedown);
                $(canvas).mousemove(handler.mousemove);

            
            }

        }

        // helpers for figuring out where to draw arrows (thanks springy.js)
        var intersect_line_line = function (p1, p2, p3, p4) {
            var denom = ((p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y));
            if (denom === 0) return false // lines are parallel
            var ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denom;
            var ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denom;

            if (ua < 0 || ua > 1 || ub < 0 || ub > 1) return false
            return arbor.Point(p1.x + ua * (p2.x - p1.x), p1.y + ua * (p2.y - p1.y));
        }

        var intersect_line_box = function (p1, p2, boxTuple) {
            var p3 = { x: boxTuple[0], y: boxTuple[1] },
                w = boxTuple[2],
                h = boxTuple[3]

            var tl = { x: p3.x, y: p3.y };
            var tr = { x: p3.x + w, y: p3.y };
            var bl = { x: p3.x, y: p3.y + h };
            var br = { x: p3.x + w, y: p3.y + h };

            return intersect_line_line(p1, p2, tl, tr) ||
                intersect_line_line(p1, p2, tr, br) ||
                intersect_line_line(p1, p2, br, bl) ||
                intersect_line_line(p1, p2, bl, tl) ||
                false
        }

        return that
    }

})()