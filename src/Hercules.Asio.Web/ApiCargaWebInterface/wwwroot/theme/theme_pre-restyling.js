var idiomasHeader = {
  init: function(){
    this.config();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.header = this.body.find('#header');
    this.navIdiomas = this.header.find('.nav-idioma');
    return;
  },
  comportamiento: function(){

    var lis = this.navIdiomas.children();
    var aLis = lis.children('a');

    aLis.bind('click', function(e){

      e.preventDefault();
      var item = $(this);
      var ico = item.children('.material-icons');
      var padre = item.parent();
      
      if (padre.hasClass('open')){
        padre.removeClass('open');
        ico.text('keyboard_arrow_down');
      }else{
        padre.addClass('open');
        ico.text('keyboard_arrow_up');
      }
    });

    return;
  }
};

var menuUsuarioConectado = {
  init: function(){
    this.config();
    this.comportamiento();

    return;
  },
  config: function(){
    this.body = body;
    this.header = this.body.find('#header');
    this.navConectado = this.header.find('.nav-conectado');

    return;
  },
  comportamiento: function(){

    var localBody = this.body;
    var usuario = this.navConectado.find('div.usuario');
    var mascarita = localBody.find('.mascaritaGenerica');
    var panelMenuUsuario = localBody.find('.panel-menu-usuario');
    var cerrar = panelMenuUsuario.find('.usuario .cerrar');

    usuario.bind('click', function(e){

      e.preventDefault();
      var item = $(this);
      var padre = item.parent();
      
      if (localBody.hasClass('mostrarNavUsuario')){
        localBody.removeClass('mostrarNavUsuario');
      }else{
        localBody.addClass('mostrarNavUsuario');
      }
    });

    mascarita.bind('click', function(e){
      localBody.removeClass('mostrarNavUsuario');
    });

    cerrar.bind('click', function(){
      localBody.removeClass('mostrarNavUsuario');
    });

    return;
  }
};

var menuLateral = {
  init: function(){
    this.config();
    this.comportamiento();
    this.marcarNiveles();

    return;
  },
  config: function(){
    this.body = body;
    this.header = this.body.find('#header');
    this.toggle = this.header.find('.col-toggle-menu');
    return;
  },
  comportamiento: function(){

    var localBody = this.body;
    var spanToggle = this.toggle.find('span');
    var mascarita = localBody.find('.mascaritaGenerica');
    var panelMenu = localBody.find('.panel-menu');
    var cerrar = panelMenu.find('.col-toggle-menu.cerrar');

    spanToggle.bind('click', function(e){

      e.preventDefault();
      var item = $(this);
      var padre = item.parent();
      
      if (localBody.hasClass('mostrarNav')){
        localBody.removeClass('mostrarNav');
      }else{
        localBody.addClass('mostrarNav');
      }
    });

    mascarita.bind('click', function(e){
      localBody.removeClass('mostrarNav');
    });

    cerrar.bind('click', function(){
      localBody.removeClass('mostrarNav');
    });

    return;
  },
  marcarNiveles: function () {
    var panelMenu = this.body.find('.panel-menu');
    var cuerpoPanelMenu = panelMenu.find('.body-panel');
    var navegacion = cuerpoPanelMenu.find('> nav');

    navegacion.find('li').each(function () {
      var item = $(this);
      var submenu = item.find('> ul');

      if(submenu.size() === 0) return;

      item.find('> a').addClass('strongItem');
    });

    return;
  }
};

var menuCreacionFlotante = {
	init: function(){
		this.config();
		this.comportamiento();
		return;
	},
	config: function(){
		this.body = body;
		return;
	},
	comportamiento: function(){

		var menu = this.body.find('.menuCreacionFlotante');
		var a = menu.find('.btn-floating');
		a.bind('click', function(e){
			e.preventDefault();
			if (menu.hasClass('active')){
				menu.removeClass('active');
			}else{
				menu.addClass('active');
			}
		});

		return;
	}
};

var mostrarOcultarFacetas = {
  init: function(){
    this.config();
    this.mostrarOcultar();
    return;
  },
  config: function(){
    this.body = body;
    return;
  },
  mostrarOcultar: function(){

    var localBody = this.body;
    var headerListado = localBody.find('.header-listado');
    var btnFacetas = headerListado.find('.boton-facetas');
    var iconoFacetas = localBody.find('.header-facetas .icono-facetas i');
    var mascarita = localBody.find('.mascaritaGenerica');

    btnFacetas.bind('click', function(e){
      e.preventDefault();
      if (localBody.hasClass('mostrarFacetas')){
        localBody.removeClass('mostrarFacetas');
        iconoFacetas.text('filter_list');
      }else{
        localBody.addClass('mostrarFacetas');
        iconoFacetas.text('close');
      }
    });

    iconoFacetas.bind('click', function(e){
      btnFacetas.trigger('click');
    });

    mascarita.bind('click', function(e){
      localBody.removeClass('mostrarFacetas');
      iconoFacetas.text('filter_list');
    });

    return;
  }
};

var cabeceraFicha = {
	init: function () {
		this.config();
		this.comportamiento();
    this.comportamientoAnclas();
    this.comportamientoActivos();
		return;
	},
	config: function () {
		this.body = body;
		return;
	},
	comportamiento: function () {
		var that = this;
		$(window)
			.scroll(function () {
				var scrollTop = $(window).scrollTop();
        var esMovil = ($(window).width() < 992) ? true : false;
        var alturaDown = (esMovil) ? 50 : 155;

        if (scrollTop < alturaDown) {
          that.body.removeClass('mostrarHeaderFicha');
        } else {
          that.body.addClass('mostrarHeaderFicha');
        }
			});

		return;
  },
  comportamientoAnclas: function(){

    var containerMain = this.body.find('.container[role="main"]');
    var wrapCol01 = containerMain.find('.wrap-col01');

    var headerFicha = this.body.find('.header-ficha');
    var nav = headerFicha.find('nav');
    var aNav = nav.find('a[data-rel]');

    var header = this.body.find('#header');
    var headerH = header.height();

    aNav.bind('click', function(e){

      e.preventDefault();
      var item = $(this);
      var li = item.parent();
      var rel = item.data('rel');

      var destino = wrapCol01.find('[data-rel="' + rel + '"]');
      var ajuste = headerH - (headerFicha.is(':visible')) ? headerFicha.height() : 0;

      if (destino.size() > 0){
        $('html, body').animate({
          scrollTop: destino.position().top - ajuste
        }, 600);
      }
    });

    return;
  },
  comportamientoActivos: function(){

    var containerMain = this.body.find('.container[role="main"]');
    var wrapCol01 = containerMain.find('.wrap-col01');
    var divsEscenas = wrapCol01.find('div[data-rel]');

    var headerFicha = this.body.find('.header-ficha');
    var nav = headerFicha.find('nav');
    var aNav = nav.find('a[data-rel]');

    var controlFicha = new ScrollMagic.Controller();

    divsEscenas.each(function(){
      var div = $(this);
      var rel = div.data('rel');
      new ScrollMagic.Scene({triggerElement: "div[data-rel='" + rel + "']", triggerHook: '0.28'})
        .addTo(controlFicha)
        //.addIndicators()
        .on("enter leave", function(e){
          aNav.removeClass('activo');
          var a = nav.find('a[data-rel="' + rel + '"]').addClass('activo');
      });
    });

    return;
  }
};

var bloqueTabs = {
  init: function(){
    this.config();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.bloqueTabs = this.body.find('.bloque-tabs');
    return;
  },
  comportamiento: function(){

    var wrapTabs = this.bloqueTabs.find('.wrap-tabs');
    var tabs = wrapTabs.find('.tabs');
    var paneles = wrapTabs.find('.paneles');

    var aTabs = tabs.find('a');

    aTabs.bind('click', function(e){

      e.preventDefault();
      var item = $(this);
      var li = item.parent();
      var rel = li.data('rel');
      var panelRel = paneles.find('.panel[data-rel="' + rel + '"]');

      tabs.find('li').removeClass('activo');
      paneles.find('.panel').removeClass('activo');
      li.addClass('activo');
      panelRel.addClass('activo');

    });

    return;
  }
};

var operativaFullWidth = {
  init: function () {
    this.config();
    this.lanzar();
    this.escalado();
    return;
  },
  config: function () {
    this.body = body;
    this.fullwidthrow = this.body.find('.fullwidthrow');
    return;
  },
  lanzar: function () {
    var windows_width = $(window).width();
    var container_width = $('.container[role="main"]').innerWidth();
    var margen = (windows_width - container_width + 30) / 2;
    var margenNegativo = parseInt('-' + margen);
    this.fullwidthrow.each(function () {
      var item = $(this);
      var hijos = item.find('> *');
      var container = $('<div />').addClass('container');
      item.css({
        "transform" : "translateX(" + margenNegativo + "px)",
        "width" : "100vw"
      });
      if(item.find('> .container').length <= 0){				
        container.append(hijos);
        item.prepend(container);
      }else{
        return true;	
      }
    });
    return;
  },
  escalado: function () {
    var that = this;
    $(window).resize(function () {
      that.lanzar();
    });			
    return;
  }
};

var customizarAvisoCookies = {
	init: function(){
		this.config();
		if (this.divCookies.size() < 1 ) return;
		this.crearXcerrar();
		return;
	},
	config: function(){
		this.body = body;
		this.divCookies = this.body.find('#phAvisoCookie');
		
		if (this.divCookies.size() < 1 ) return;
		
		this.divInfo = this.divCookies.find('.info');
		this.title = this.divInfo.find('.title-panel-info');
		this.texto = this.divInfo.find('.col-4');
		return;
	},
	crearXcerrar: function(){
		var that = this;
		var cerrar = $('<a />').attr('href', '#').addClass('cerrar').text('x');
		cerrar.on('click', function(e){
			e.preventDefault();
			that.divCookies.slideUp(750, function(){
				that.divCookies.remove();
			})
		})
		this.divInfo.append(cerrar);
		return;
	}
};

var misSuscripcionesHomeConectado = {
	init: function(){
		this.config();
		this.eventos();
		this.peticion();
		return;
	},
	config: function(){
		this.body = body;
		this.col02 = this.body.find('#col02');
		this.tabs = this.col02.find('.tabs');
		this.panelesTabs = this.col02.find('.panelesTab');
		return;
	},
	eventos: function(){

		var paneles = this.panelesTabs;
		var componentes = paneles.children();
		var lis = this.tabs.find('li');

		lis.bind('click', function(e){

			var li = $(e.target);
			var clase = li.attr('class').replace('activo', '').trim();

			var componente = paneles.find('.' + clase);

			lis.removeClass('activo');
			componentes.removeClass('activo');
			li.addClass('activo');
			componente.addClass('activo');

		});

		return;
	},
	peticion: function(){

		var that = this;
		var paneles = this.panelesTabs;
		var componenteSuscripciones = paneles.find('.componenteSuscripciones');
    	var url = comunidadName + "/CMSpagina?ComponentName=ncorto_misSuscripciones";
    	var respuesta = $.ajax({
            url: url,
            type: "GET",
            cache: false,
            dataType: "html"
        })
        .done(function(respuesta) {
            var componente = $(respuesta);

            paneles.find('.preloader-wrapper').hide();
            componenteSuscripciones.append(componente);

            that.comprobarSuscripciones();

            return;
        })
        .fail(function(jqXHR, textStatus) {
            $.each(jqXHR, function(index, value) {});
        });	

		return;
	},
	comprobarSuscripciones: function(){

		var liActividadReciente = this.tabs.find('li.actividadReciente');
		var liSuscripciones = this.tabs.find('li.suscripciones');
		var suscripciones = this.panelesTabs.children('.suscripciones');
		var actividadReciente = this.panelesTabs.children('.actividadReciente');

		var sinResultados = this.panelesTabs.find('.sinResultados').size() > 0;

		if (sinResultados){
			liSuscripciones.removeClass('activo');
			suscripciones.removeClass('activo');
			liActividadReciente.addClass('activo');
			actividadReciente.addClass('activo');
		}else{
			liActividadReciente.removeClass('activo');
			actividadReciente.removeClass('activo');
			liSuscripciones.addClass('activo');
			suscripciones.addClass('activo');
		}

		return;
	}
};

var montarMenuHeaderFicha = {
  init: function(){
    this.config();
    this.montar();
    this.anclaComentarios();
    return;
  },
  config: function(){
    this.body = body;
    this.wrapCol01 = this.body.find('.wrap-col01');
    return;
  },
  montar: function(){

    var h2 = this.wrapCol01.find('h2');
    var headerFicha = this.body.find('.header-ficha');
    var navHeaderFicha = headerFicha.find('nav');

    var cont = 0;

    h2.each(function(){

      var item = $(this);
      var padre = item.parent();
      var dataRel = padre.data('rel');

      if (dataRel != undefined && !padre.hasClass('creado-enlace')){

        padre.addClass('creado-enlace');

        var txtEnlace = item.text();
        if (txtEnlace == "Documentos adjuntos") txtEnlace = "Documentos";

        var li = $('<li />');
        var a = $('<a />').attr('data-rel', dataRel).text(txtEnlace);

        if (dataRel == "inicio") a.addClass('activo');

        li.append(a);
        navHeaderFicha.append(li);

        cont++;

      }

      if (cont == 0){
        headerFicha.find('.wrap').addClass('sin-nav');
      }

    });

    return;
  },
  anclaComentarios: function(){
    var anclaComentarios = this.body.find('.anclaComentarios');
    var comentarios = this.wrapCol01.children('.comments');
    var header = this.body.find('#header');

    anclaComentarios.bind('click', function(e){

      e.preventDefault();
      var headerH = header.height();
      var pos = comentarios.position().top - headerH;
    
      $('html, body').animate({
        scrollTop: pos
      }, 600);

    });

    return;
  }
};

var desplegablesSlideHome = {
  init: function(){
    this.config();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.portada = this.body.find('.portadaHome');
    return;
  },
  comportamiento: function(){

    var botones = this.portada.find('.botones');
    var wrapsLi = botones.find('.wrap-li');
    var aBotones = wrapsLi.children('a');
    var icos = aBotones.find('.material-icons');

    aBotones.bind('click', function(e){

      e.preventDefault();
      var item = $(this);
      var ico = item.find('.material-icons');
      var wrapLi = item.parent();

      if (wrapLi.hasClass('abierto')){
        wrapLi.removeClass('abierto');
        ico.text('keyboard_arrow_down');
      }else{
        wrapsLi.removeClass('abierto');
        wrapLi.addClass('abierto');
        icos.text('keyboard_arrow_down');
        ico.text('close');
      }

    });

    return;
  }
};

var cambioVistaListados = {
  init: function(){
    this.config();
    this.cambio();
    return;
  },
  config: function(){
    this.body = body;
    return;
  },
  cambio: function(){

    var panResultados = this.body.find('#panResultados');
    var cambioVista = this.body.find('.cambio-vista');
    var lis = cambioVista.find('li');
    var aLis = lis.find('a');

    aLis.bind('click', function(e){

      e.preventDefault();
      var item = $(this);
      var li = item.parent();
      var clase = li.attr('class').replace('activeView', '').trim();

      lis.removeClass('activeView');
      li.addClass('activeView');
      panResultados.removeAttr('class');
      panResultados.addClass(clase);

    });


    return;
  }
};

var activarPalco = {
  init: function(){
    this.config();
    this.activar();
    this.cerrar();
    return;
  },
  config: function(){
    this.body = body;
    this.palco = this.body.find('#palco');
    return;
  },
  activar: function(){

    var localBody = this.body;
    var rowCabeceraFicha = localBody.find('.row-cabecera-ficha');
    var btnIr = rowCabeceraFicha.find('.btn-iralaweb');
    var iframe = this.palco.find('iframe');
    var url = btnIr.attr('href');
    var h1 = rowCabeceraFicha.find('h1');

    if (url != undefined && url.length > 0){
      var schema = undefined;
      if (url.indexOf(':') != -1) {
        schema = url.split(':')[0] + '://';
      }

      if (typeof(schema) != 'undefined' && (window.location.href.startsWith(schema) || window.location.href.startsWith('http://'))){
        btnIr.bind('click', function(e){
          e.preventDefault();
          if (url != undefined && url != ""){
            iframe.attr('src', url);
            localBody.addClass('palcoActivo');
          }
        });

        h1.bind('click', function(e){
          e.preventDefault();
          e.stopPropagation();
          btnIr.trigger('click');
        });
        
      }else{
        btnIr.bind('click', function(e){
          e.preventDefault();
          window.open(btnIr.attr('href'));
        });
      }
    }else{
      btnIr.bind('click', function(e){
        e.preventDefault();
        window.open(btnIr.attr('href'));
      });
    }

    return;
  },
  cerrar: function(){

    var localBody = this.body;
    var cerrar = this.palco.find('.col-cerrar span');

    cerrar.bind('click', function(e){
      localBody.removeClass('palcoActivo');
    });

    return;
  }
};

var anclasGestionEducativa = {
  init: function(){
    this.config();
    this.ancla();
    return;
  },
  config: function(){
    this.body = body;
    return;
  },
  ancla: function(){

    var url = window.location.href;
    var pos = url.indexOf('#');

    var header = this.body.find('#header');
    var headerH = header.height();

    if (pos > -1){
      var id = url.substring(pos + 1);
      var destino = this.body.find('#' + id);

      if (destino.size() > 0){
        $('html, body').animate({
          scrollTop: destino.position().top - headerH
        }, 600);
      }
    }

    return;
  }
};

var comprobarAnclaFicha = {
  init: function(){
    this.config();
    this.ancla(); 
    return;
  },
  config: function(){
    this.body = body;
    return;
  },
  ancla: function(){

    var url = window.location.href;
    var pos = url.indexOf('#comments');

    var header = this.body.find('#header');
    var headerH = header.height();

    if (pos > -1){
      var destino = this.body.find('.wrap-col01 > .comments');

      if (destino.size() > 0){
        $('html, body').animate({
          scrollTop: destino.position().top - headerH
        }, 600);
      }
    }

    return;
  }
};

var buscarCabeceraMovil = {
  init: function(){
    this.config();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.header = this.body.find('#header');
    return;
  },
  comportamiento: function(){

    var localBody = this.body;
    var colBuscar = this.header.find('.col-buscar-movil');
    var ico = colBuscar.find('span');

    colBuscar.bind('click', function(){

      if (localBody.hasClass('buscarCabeceraMovil')){
        localBody.removeClass('buscarCabeceraMovil');
        ico.text('search');
      }else{
        localBody.addClass('buscarCabeceraMovil');
        ico.text('close');
      }

    });

    return;
  }
};

var desplegarNavHomeListadosMovil = {
  init: function(){
    this.config();
    this.desplegar();
    return;
  },
  config: function(){
    this.body = body;
    return;
  },
  desplegar: function(){

    var cabeceraListados = this.body.find('.cabecera-listados');
    var navMovil = cabeceraListados.find('.nav-movil');
    var desplegarNav = navMovil.find('.desplegar-nav');

    desplegarNav.bind('click', function(){
      if (cabeceraListados.hasClass('open')){
        cabeceraListados.removeClass('open');
        desplegarNav.text('keyboard_arrow_down');
      }else{
        cabeceraListados.addClass('open');
        desplegarNav.text('keyboard_arrow_up');
      }
    });


    return;
  }
};

var customizarComentarios = {
	init: function(){
		this.config();
		//this.crearEstructuraComentariosDosDivs();
		this.customizar();
		this.funcionalidadVerTodos();
		this.anclaBarraEdicion();
		return;
	},
	config: function(){
		this.body = body;
		this.componente = this.body.find('.resource-comments');
		this.comentarios = this.componente.find('.comment');
		this.panComentarios = this.componente.find('#panComentarios');
		this.desplegarComentarios = this.componente.find('.desplegarComentarios');
		this.numComentarios = this.comentarios.size();
		return;
	},
	customizar: function(){
		var that = this;
		this.comentarios.each(function(numero){
			var comentario = $(this);
			var autor = comentario.find('.author').first();
			var fecha = autor.next().addClass('comment-date');
			autor.append(fecha);
			that.organizarComentarios(comentario, numero);
		})
		return;
	},
	crearEstructuraComentariosDosDivs: function(){
		var defaultComments = $('<div />').addClass('defaultComments');
		var allComments = $('<div />').addClass('allComments');

		this.panComentarios.append(defaultComments);
		this.panComentarios.append(allComments);

		return;
	},	
	organizarComentarios: function(item, numeroItem){
		if(numeroItem >= 2 ){
			item.addClass('desplegable');
		}

		return;
	},
	funcionalidadVerTodos: function(){
		var that = this;
		
		if (this.desplegarComentarios.size() == 0 && this.numComentarios > 2){
			this.crearDesplegarComentarios();
		}
		
		this.desplegarComentarios.bind('click', function(event) {
			if(that.panComentarios.hasClass('mostrarTodos')){
				that.panComentarios.removeClass('mostrarTodos');
			}else{
				that.panComentarios.addClass('mostrarTodos');
			}
		});
		
		return;
	},
	crearDesplegarComentarios: function(){
		var spanIco = $('<span />').addClass('icono');
		var spanLiteral = $('<span />').addClass('literal').text('Ver todos').append(spanIco);
		var despComentarios = $('<div />').addClass('desplegarComentarios').append(spanLiteral);
		
		var boxComments = this.componente.find('.box.comments');
		boxComments.append(despComentarios);
		this.desplegarComentarios = despComentarios;
		return;
	},
	anclaBarraEdicion: function(){

		if (this.body.hasClass('SharableLearningObjectResource') || this.body.hasClass('StructuredLearningObjectResource')) return;

		var barraEdicion = this.body.find('.componenteAutorUtilsAccionesRedes');
		var liComentarios = barraEdicion.find('li.comentarios');

		var comentarios = this.body.find('#comments');
		var posCommentarios = comentarios.offset().top;

		var header = this.body.find('#header');
		var headerMovil = this.body.find('#headerMovil');


		liComentarios.bind('click', function(){

			if (header.is(':visible')){
				posCommentarios -= header.height();
			}else{
				posCommentarios -= headerMovil.height();
			}

			$('html, body').animate({
				scrollTop: posCommentarios
			}, 600)

		});

		return;
	}
};

var mostrarAccionesSocialesVersionResponsivaComentarios = {
  init: function () {
    this.config();
    this.mostrarAccionesConClick();
    this.cambiarEntreAccionesSocialesEscritorioYMovil();

    return;
  },
  config: function () {
    this.body = body;
    this.comentarios = this.body.find('.comments');
    this.cabeceraComentario = this.comentarios.find('.commentHead');
    this.lanzador = this.cabeceraComentario.find('.verAccionesSociales');

    this.acciones = this.cabeceraComentario.find('.accionesSociales');
    this.icono = this.lanzador.find('.icono');

    return;
  },
  mostrarAccionesConClick: function () {

    var that = this;

    this.lanzador.each(function () {

      var lanzador = $(this);

      lanzador.bind('click', function (evento) {
        evento.preventDefault();

        var iconosSociales = lanzador.parent().find('.accionesSociales');
        var icono = lanzador.find('span.icono');
        var anchoDiv = lanzador.parent().width();

        if (iconosSociales.is(':visible')) {
          icono.css('transform', 'rotate(90deg)');
          iconosSociales.hide();
          //iconosSociales.css('width', 0);
        } else {
          icono.css('transform', 'rotate(45deg)');
          iconosSociales.show();
          iconosSociales.css('width', anchoDiv - 100);
        }
      })

    })

    return;
  },
  cambiarEntreAccionesSocialesEscritorioYMovil: function () {
    var ventana = $(window);
    var that = this;

    ventana.resize(function () {
      if (ventana.width() > 700) {
        that.acciones.show();
        that.acciones.css('width', 'auto');
      }
    });

    return;
  }
};

var verMasRowCreditosFicha = {
  init: function(){
    this.config();
    this.ocultarPrimero();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.row = this.body.find('.row-creditos, .row-info-curricular');
    return;
  },
  ocultarPrimero: function(){
 
    var verMas = this.row.find('.verMas');

    verMas.each(function(i){
      var ico = $(this);
      var padre = ico.parents('p').first();
      var spanContenedor = padre.children('span');
      spanContenedor.append(ico);

      var items = spanContenedor.children();
      var totalItems = items.size();
      if (totalItems > 2){
        items.each(function(i){
          var itm = $(this);
          if (i > 2) itm.hide();
          if (i == 2) itm.addClass('ultimo');
        });
        padre.addClass('mas');
        ico.text('add');
      }else{
        ico.hide();
      }
    });

    return;
  },
  comportamiento: function(){

    var verMas = this.row.find('.verMas');

    verMas.unbind('click').bind('click', function(e){
      e.preventDefault();
      var ico = $(this);
      var padre = ico.parents('p').first();
      var spanContenedor = padre.children('span');

      var items = spanContenedor.children();
      var totalItems = items.size();

      if (padre.hasClass('mas')){
        items.show().removeClass('ultimo');
        padre.removeClass('mas');
        ico.text('remove');
      }else{
        items.each(function(i){
          var itm = $(this);
          if (i > 2 && i <= totalItems - 1) itm.hide();
          if (i == 2) itm.addClass('ultimo');
        });
        padre.addClass('mas');
        ico.text('add');
      }

    });

    return;
  }
};

var datatableFichaHistorial = {
  init: function(){
    this.config();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.boxHistorial = this.body.find('.box.historial');
    return;
  },
  comportamiento: function(){

    var tabla = this.boxHistorial.find('table');

    tabla.DataTable({
      paging: false,
      info: false,
      ordering: false,
      searching: false,
      autoWidth: true,
      responsive: true
    });

    return;
  }
};

var headerMinScroll = {
	posicionAnterior: 0,
	obj_top: 0,
	init: function () {
		this.config();
		this.scroll();
		return;
	},
	config: function () {
		this.body = body;
		return;
	},
	scroll: function () {
		var that = this;
		$(window).scroll(function () {
			that.lanzar();
		});
		return;
	},
	lanzar: function () {
		var obj = $(window);
		this.posicionAnterior = this.obj_top;
		this.obj_top = obj.scrollTop();
		if (this.obj_top < this.posicionAnterior || this.obj_top <= 10) {
			this.body.removeClass("headerMin");
		} else {
			this.body.addClass("headerMin");
		}
		return;
	}
};

var comportamientoMenuResponsive = {
  init: function () {
    this.config();
	  this.body.addClass('calculandoMenu');
    this.comportamiento();
    this.cambiarClaseActivaSegundoNivel();	
    if ($(window).width() > 992){
	    if($('.row-menu nav .active-nivel-2').length){
			  $('.row-menu nav > li.active').insertBefore($('.row-menu nav > li.active').prev());
			  comportamientoMenuResponsive.init();
	    }
    }
	  this.body.removeClass('calculandoMenu');

    return;
  },
  config: function () {
    this.body = body;
    this.header = this.body.find('#header');
    this.rowMenu = this.header.find('.row-menu');
    this.menuPrincipal = this.rowMenu.find('.col');
    this.ulMenuPrincipal = this.menuPrincipal.find('nav');
    this.dropdownMenu = this.ulMenuPrincipal.find('.dropdown-menu');
    this.maxWidthMenu = (this.menuPrincipal.width() < 1) ? this.menuPrincipal.parents('.row').width() - 150 : this.menuPrincipal.width() - 150;

    return;
  },
	cambiarClaseActivaSegundoNivel: function(){
		var itemActivo = this.dropdownMenu.find('.active');
		itemActivo.removeClass('active').addClass('active-nivel-2');
		return;
	},
  comportamiento: function () {
    var that = this;
    var totalWidth = 0;
    var totalWidthListado = this.maxWidthMenu;
    this.ulMenuPrincipal.show();
    this.dropdownMenu.empty();
    
    this.ulMenuPrincipal.children('li').removeClass('oculto');
    this.ulMenuPrincipal.children('li').each(function (indice) {
      var item = $(this);
      if(item.hasClass('dropdown')) return;
      totalWidth = item.innerWidth() + totalWidth;
      if (totalWidthListado > totalWidth) return;
      that.dropdownMenu.append(item.clone());
      item.addClass('oculto');
    });	
    if(that.dropdownMenu.children().length==0){
      that.dropdownMenu.parent('.dropdown').addClass('oculto');
    }      
    return;
  }
};

/* COLORES */

var sacarPrimerasLetrasNombre = {
	init: function(numLetras, nombre){
		var resul = this.sacar(numLetras, nombre);
		return resul;
	},
	sacar: function(numLetras, nombre){
		var resul = "";
		if (nombre == undefined) return;		
		var partes = nombre.split(' ');
		$.each(partes, function(c,v){
			if (c > numLetras - 1) return false;
			var primera = v.substring(0, 1);
			resul = resul + primera;
		});
		
		return this.sustituirAcentos(resul);
	},
	sustituirAcentos: function(text) {
	    var acentos = "ÃÀÁÄÂÈÉËÊÌÍÏÎÒÓÖÔÙÚÜÛãàáäâèéëêìíïîòóöôùúüûÑñÇç";
	    var original = "AAAAAEEEEIIIIOOOOUUUUaaaaaeeeeiiiioooouuuunncc";
	    for (var i=0; i<acentos.length; i++) {
	        text = text.replace(acentos.charAt(i), original.charAt(i));
	    }
	    return text;
	}
};

var obtenerClaseBackgroundColor = {
	init: function(nombre){
		var resul = this.obtener(nombre);
		return resul;
	},
	obtener: function(nombre){
		//var number = Math.floor(Math.random() * maximo) + 1;
		if (nombre == undefined) return;
		var letra = sacarPrimerasLetrasNombre.init(1, this.sustituirAcentos(nombre)).toLowerCase();
		return 'color-' + letra;
	},
	sustituirAcentos: function (text) {
		if (text == null) return;
		var acentos = "ÃÀÁÄÂÈÉËÊÌÍÏÎÒÓÖÔÙÚÜÛãàáäâèéëêìíïîòóöôùúüûÑñÇç";
		var original = "AAAAAEEEEIIIIOOOOUUUUaaaaaeeeeiiiioooouuuunncc";
		for (var i=0; i<acentos.length; i++) {
			text = text.replace(acentos.charAt(i), original.charAt(i));
		}
		return text;
	}
};

var colorearCirculos = {
	init: function(){
		this.config();
		this.colorear();
		return;
	},
	config: function(){
		this.body = body;
		this.circulos = this.body.find('div.circulo');
		return;
	},
	colorear: function(){

    this.circulos.each(function(){

      var circulo = $(this);
      var span = circulo.find('span');
      var style = circulo.attr('style');

      if (style == undefined){

        var iniciales = span.text();
        var clase = obtenerClaseBackgroundColor.init(iniciales);
        circulo.addClass(clase);

      }
    });

		return;
	}
};

var engancharBuscarHeader = {
  init: function(){
    this.config();
    this.enganchar();
    return;
  },
  config: function(){
    this.body = body;
    this.header = this.body.find('#header');
    return;
  },
  enganchar: function(){

    var formBuscador = this.header.find('#formBuscador');
    var buscar = formBuscador.find('.material-icons.buscar');
    var buscarOrig = formBuscador.find('#btnBuscarPrincipal');

    buscar.bind('click', function(){
      buscarOrig.trigger('click');
    });

    return;
  }
};

var customizarEditarRecurso = {
  init: function(){
    this.config();
    this.customizar();
    return;
  },
  config: function(){
    this.body = body;
    this.tituloPag = this.body.find('#lblTituloPagina');
    this.formSemEdicion = this.body.find('.formSemEdicion');
    return;
  },
  customizar: function(){

    if (this.formSemEdicion.hasClass('formSemEdicion_EducationalExperience')){
      this.customizarExperience();
    }else if (this.formSemEdicion.hasClass('formSemEdicion_EducationalSpace')){
      this.customizarSpace();
    }

    return;
  },
  customizarExperience: function(){

    var txtTitulo = this.tituloPag.text();
    this.tituloPag.text(txtTitulo + ': Experiencia innovadora');

    this.moverEtiquetas();

    return;
  },
  customizarSpace: function(){

    var txtTitulo = this.tituloPag.text();
    this.tituloPag.text(txtTitulo + ': Espacio educativo');

    this.moverEtiquetas();

    return;
  },
  moverEtiquetas: function(){

    var divEtiquetas = this.body.find('#SubirRecurso fieldset.labels');
    var divDescription = this.body.find('.cont_description');

    divEtiquetas.insertAfter(divDescription);

    return;
  }
};

var enlacesFichas = {
  init: function(){
    this.config();
    this.comprobar();
    return;
  },
  config: function(){
    this.body = body;
    this.divEnlaces = this.body.find('.row-enlaces');
    return;
  },
  comprobar: function(){

    var enlaces = this.divEnlaces.find('a');
    var hrefTitulo = this.body.find('h1 a').attr('href');
    var btnIr = this.body.find('.btn-iralaweb');

    /*
    enlaces.each(function(){
      var a = $(this);
      var href = a.attr('href');

      if (href != undefined && href.length > 0){
        var schema = undefined;
        if (href.indexOf(':') != -1) {
          schema = href.split(':')[0] + '://';
        }
        if (typeof(schema) != 'undefined' && (window.location.href.startsWith(schema) || window.location.href.startsWith('http://'))){
          if (btnIr.size() > 0) {
            a.bind('click', function(e){
              e.preventDefault();
              btnIr.trigger('click');
            });
          }
        }

      }

      if (hrefTitulo == href){

      }

    });
    */

    return;
  }
};

var backgroundHome = {
  init: function(){
    this.config();
    this.poner();
    return;
  },
  config: function(){
    this.body = body;
    this.portada = this.body.find('.portadaHome');
    return;
  },
  poner: function(){
    
    var urlBackground = this.portada.find('.urlBackground');

    if (urlBackground.length > 0) this.portada.css('background', 'url(' + urlBackground.text().trim() + ') no-repeat center');

    return;
  }
};

var recortarQueEstaPasandoHomeDesconectado = {
  init: function(){
    this.config();
    this.recortar();
    return;
  },
  config: function(){
    this.body = body;
    this.que = this.body.find('.que-esta-pasando');
    return;
  },
  recortar: function(){

    var resources = this.que.find('.resource');
    var desc = resources.find('.desc');

    desc.dotdotdot({
      height: 80
    });

    return;
  }
};

/* COOKIES */

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays*24*60*60*1000));
    var expires = "expires="+ d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
};

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for(var i = 0; i <ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
};

/* COMPLETADA CARGA */
//var comportamientoCargaFacetasComunidad = function(){
  //personalizarFacetas.init();
  //marcarFacetasAplicadas.init();
//}

function CompletadaCargaSubidaArchivo_SemCms(){

  if (body.hasClass('editarRecurso')){
    var fileFormSem = body.find('.fileFormSem');
    var inputImgPrincipal = body.find('input[id^="chkImgPrincfileUpLoad"]');

    fileFormSem.each(function(){

      var item = $(this);
      var a = item.children('a');
      var onclick = a.attr('onclick');

      if (onclick != undefined){
        if (onclick.indexOf('imgfileUpLoad') > -1){
          //inputImgPrincipal.prop('checked', true);
          inputImgPrincipal.trigger('click');
        }
      }

    });
  }

}

function CompletadaCargaRecursosComunidad(){
  colorearCirculos.init();
}

/* SOBREESCRIBIR GNOSS */
/*
function ObtenerEntidadesLOD(pUrlServicio, pUrlBaseEnlaceTag, pDocumentoID, pEtiquetas, pIdioma) {
  var servicio = new WS(pUrlServicio, WSDataType.jsonp);

  var metodo = 'ObtenerEntidadesLOD';
  var params = {};
  params['documentoID'] = pDocumentoID;
  params['tags'] = pEtiquetas;
  params['urlBaseEnlaceTag'] = pUrlBaseEnlaceTag;
  params['idioma'] = pIdioma;
  servicio.call(metodo, params, function (data) {
      $('.listTags').find('a').each(function () {
          var tag = this.textContent;
          if (data[tag] != null) {
              $(this).attr('title', data[tag]);
              $(this).addClass('conFbTt');
          }
      });
      $(".conFbTt").each(function () {
          if (this.title) {
              this.tooltipData = this.title;
              this.removeAttribute('title');
          }
      }).hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
  });
}
*/


var body, comunidadName;

$(function(){

  body = $('body');
  comunidadName = body.find('#inpt_baseUrlBusqueda').attr('value');

  operativaFullWidth.init();
  idiomasHeader.init();
  menuCreacionFlotante.init();
  menuUsuarioConectado.init();
  menuLateral.init();
  colorearCirculos.init();
  engancharBuscarHeader.init();
  buscarCabeceraMovil.init();
  headerMinScroll.init();
  //comportamientoMenuResponsive.init();  

  customizarAvisoCookies.init();

  if (body.hasClass('listadoComunidad')){
    mostrarOcultarFacetas.init();
    cambioVistaListados.init();
  }else if (body.hasClass('homeListados')){

    desplegarNavHomeListadosMovil.init();
    if (body.hasClass('homeGestionEducativa')){
      anclasGestionEducativa.init();
      bloqueTabs.init();
      comprobarAnclaGestionEducativa.init();
    }

  }else if (body.hasClass('homeComunidad')){
    if (body.hasClass('usuarioRegistrado')){
      misSuscripcionesHomeConectado.init();
    }else if (body.hasClass('invitado')){
      backgroundHome.init();
      desplegablesSlideHome.init();
    }
    recortarQueEstaPasandoHomeDesconectado.init();
  }else if (body.hasClass('ficha')){
    montarMenuHeaderFicha.init();
    cabeceraFicha.init();
    activarPalco.init();
    comprobarAnclaFicha.init();
    customizarComentarios.init();
    mostrarAccionesSocialesVersionResponsivaComentarios.init();
    verMasRowCreditosFicha.init();
    enlacesFichas.init();
  }else if (body.hasClass('editarRecurso')){
    customizarEditarRecurso.init();
  }else if (body.hasClass('fichaHistorial')){
    datatableFichaHistorial.init();
  }

});

var lanzarRegistroAccionInvitado = {
	init: function(){
		window.location.href = $('.nav-desconectado li a').attr("href");
		return;
	}
}