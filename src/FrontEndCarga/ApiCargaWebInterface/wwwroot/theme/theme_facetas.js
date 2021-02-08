
function comportamientoCargaFacetasComunidad() {
  //verFacetasMovil.init();
  //comportamientoFacetasPopUp.init();
  //personalizarFacetas.init();
  comportamientoFacetas.init();
  $('.mas-menos .ver-mas').unbind().click(function () {
    var padre = $(this).parents('.mas-menos');
    $(padre).find('ul.listadoFacetas>.oculto').show();
    $(padre).find('.opciones .oculto').show();
    $(padre).find('.ver-mas').hide();
    return false;
  });
  $('.mas-menos .ver-menos').unbind().click(function () {
    var padre = $(this).parents('.mas-menos');
    $(padre).find('ul.listadoFacetas>.oculto').hide();
    $(padre).find('.opciones .oculto').hide();
    $(padre).find('.ver-menos').hide();
    $(padre).find('.ver-mas').show();
    return false;
  });
  $('.desplegarSubFaceta .material-icons').unbind().click(function () {
    var padre = $($(this).parents('li')[0]);
    if ($(this).text() == 'keyboard_arrow_down') {
      $($(padre).find('ul')[0]).show();
      $(this).text('keyboard_arrow_up');
    } else {
      $($(padre).find('ul')[0]).hide();
      $(this).text('keyboard_arrow_down');
    }
    return false;
  });
  //alturizarBodyTamanoFacetas.init();
  //'Arregglamos' la faceta tipo de noticia
  tipoNoticia = [
    ['simple', 'Actualidad', 'Topical issues'],
    ['video', 'Vídeo', 'Video'],
    ['event', 'Evento', 'Event'],
    ['infografia', 'Infografía', 'Infography'],
    ['fotogaleria', 'Fotogalería', 'Photogallery'],
    ['kit', 'Kit de prensa', 'Press kit'],
    ['data', 'Datos', 'Data'],
    ['storytelling2', 'Storytelling', 'Storytelling'],
    ['storytelling', 'Storytelling', 'Storytelling']
  ];
  var langActual = $('input.inpt_Idioma').val();
  $('#ernews--format li a .textoFaceta,#panListadoFiltros li').each(function () {
    for (i = 0; i < tipoNoticia.length; i++) {
      var nombre = tipoNoticia[i][0];
      if ($(this).html().indexOf(nombre) == 0) {
        if (langActual == 'es') {
          $(this).html($(this).html().replace(nombre, tipoNoticia[i][1]));
        } else {
          $(this).html($(this).html().replace(nombre, tipoNoticia[i][2]));
        }
        break;
      }
    }
  });
  if ($('#panListadoFiltros li').length) {
    $('#panFiltros .borrarFiltros').show();
  } else {
    $('#panFiltros .borrarFiltros').hide();
  }


  MontarResultadosScroll.init('#footer','.resource');	
  MontarResultadosScroll.CargarResultadosScroll = function (pData) {
    var htmlRespuesta = document.createElement("div");
    htmlRespuesta.innerHTML = pData;
    $(htmlRespuesta).find('.resource').each(function () {
        $('#panResultados .resource').last().after(this)
    });
  }

};

var comportamientoFacetas = {
  init: function () {
    this.config();
    return;
  },
  config: function () {
    var that = this;
    $('#panFacetas .faceta-title').unbind().click(function (e) {
      var facetaAct = $(this).parent();
      //cargar faceta si no tiene contenido
      that.cargarFaceta(facetaAct);
    });
  },
  cargarFaceta: function (faceta) {
    if (faceta.find('ul li').length == 0) {
      var idFaceta = faceta.attr('faceta');
      var claveFaceta = replaceAll(replaceAll(idFaceta, '---', '@@@'), '--', ':');
      VerFaceta(claveFaceta, idFaceta);
    }
    this.desplegarFaceta(faceta);
  },
  desplegarFaceta: function (faceta) {
    $(faceta).find($('ul')).slideToggle('fast');
    $(faceta).toggleClass('plegado');
    setTimeout(function () { alturizarBodyTamanoFacetas.init(); }, 400);
    return;
  }
};

var personalizarFacetas = {
	init: function(){
		this.config();
		this.personalizar();
		return;
	},
	config: function(){
		this.body = body;
		this.panFacetas = this.body.find('#panFacetas');
		return;
	},
	personalizar: function(){

		var aFaceta = this.panFacetas.find('a.faceta').not('.faceta-personalizada');

		aFaceta.each(function(i){

			var item = $(this);
			var li = item.parent();
			var imgMas = li.find('.imgMas');

			var checkbox = li.children('input[type="checkbox"]');

			if (checkbox.size() < 1){
				checkbox = $('<input />').attr({
					'type' : 'checkbox',
					'name' : 'chkFaceta' + i,
					'id' : 'chkFaceta' + i
				}).addClass('filled-in');

				if (imgMas.size() > 0){
					checkbox.insertAfter(imgMas);
				}else{
					li.prepend(checkbox);
				}

				var label = $('<label />').attr('for', 'chkFaceta' + i);

				label.insertAfter(checkbox);
				label.wrapInner(item);

				li.on('click', label, function(e){
				});
			}

			if (item.hasClass('applied')) {
				checkbox.prop('checked', true);
				li.addClass('aplicada');
			}

			if (imgMas.size() > 0){
				var angle = $('<span />').addClass('angle');
				li.append(angle).addClass('conAngle');

				angle.bind('click', function(){
					imgMas.trigger('click');
				});
			}

			item.addClass('faceta-personalizada');

		});

		var aplicadas = this.panFacetas.find('li.aplicada');

		var liparent = aplicadas.parent().prev();

		liparent.each(function(){
			var item = $(this);
			if (item.get(0).nodeName == 'LI'){
				item.children('.imgMas').trigger('click');
			}
			
		});


		return;
	}
};

function enlazarFacetasBusqueda() {
  $('.facetedSearchBox .filtroFaceta')
    .unbind()
    .keydown(function (event) {
      if ($(this).val().indexOf('|') > -1) {
        $(this).val($(this).val().replace(/\|/g, ''));
      };

      if (event.which || event.keyCode) {
        if ((event.which == 13) || (event.keyCode == 13)) {
          return false;
        }
      } else {
        return true;
      };
    });

  var desde = '';
  var hasta = '';
  if ($('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker').length > 0) {
    desde = $('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker')[0].value;
    hasta = $('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker')[1].value;
  }

  $('.facetedSearchBox .searchButton')
    .unbind()
    .click(function (event) {
      if ($(this).parents('.facetedSearchBox').find('.filtroFaceta').length == 1) {
        if ($(this).parents('.facetedSearchBox').find('.filtroFacetaTesauroSemantico').length == 1 && $(this).parents('.facetedSearchBox').find('.filtroFaceta').val().indexOf('@' + $('input.inpt_Idioma').val()) == -1) {
          AgregarFaceta($(this).parents('.facetedSearchBox').find('.filtroFaceta').attr('name') + '=' + $(this).parents('.facetedSearchBox').find('.filtroFaceta').val() + '@' + $('input.inpt_Idioma').val());
        } else {
          AgregarFaceta($(this).parents('.facetedSearchBox').find('.filtroFaceta').attr('name') + '=' + $(this).parents('.facetedSearchBox').find('.filtroFaceta').val());
        }
      } else {
        var filtroBusqueda = $(this).attr('name');
        var fechaDesde = $(this).parents('.facetedSearchBox').find('input')[0];
        var fechaHasta = $(this).parents('.facetedSearchBox').find('input')[1];
        var formatoFecha = false;

        if (typeof ($(this).parents('.facetedSearchBox').find('input.hasDatepicker')[0]) != 'undefined') {
          formatoFecha = true;
        }

        if (desde == '') {
          desde = $('input.inpt_Desde').val();
        }
        if (hasta == '') {
          hasta = $('input.inpt_Hasta').val();
        }

        var filtro = ObtenerFiltroRango(fechaDesde, desde, fechaHasta, hasta, formatoFecha);

        if (filtro != '-') {
          AgregarFaceta(filtroBusqueda + '=' + filtro);
        }
      }
      return false;
    });
  $('.facetedSearch a.faceta')
    .unbind()
    .click(function (e) {
      AgregarFaceta($(this).attr("name").replace('#', '%23'));
      $('body').removeClass('mostrarFacetas');
      e.preventDefault();
    });
    
    $('.facetedSearch a.faceta.grupo')
    .unbind()
    .click(function (e) {
      AgregarFacetaGrupo($(this).attr("name").replace('#', '%23'));
      $('body').removeClass('mostrarFacetas');
      e.preventDefault();
    });
};

var limiteLongitudFacetas = {
  init: function(){return;}
};