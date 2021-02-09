/*
	Theme Name: GNOSS Front - MyGnoss base theme
	Theme URI: http://dewenir.es

	Author: GNOSS Front
	Author URI: http://dewenir.es

	Description: Fichero base de customización del tema de MyGNOSS.
	Version: 1.0
*/

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

var bodyScrolling = {
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
    this.obj_top = obj.scrollTop();
    if (this.obj_top <= 10) {
      this.body.removeClass("scrolling");
    } else {
      this.body.addClass("scrolling");
    }
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
    this.main = this.body.find('main[role="main"] > .container');
    return;
  },
  lanzar: function () {
    var windows_width = $(window).width();
    var container_width = this.main.width();
    var anchoScrollbar = (this.body.width() > 767) ? 31.5 : 0;
    var margen = (windows_width - container_width + anchoScrollbar) / 2;

    var margenNegativo = parseFloat('-' + margen);

    this.fullwidthrow.each(function () {
      var item = $(this);
      item.css({
        "transform" : "translateX(" + margenNegativo + "px)",
        "width" : "100vw"
      });
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

/*var anchoColsHeaderListado = {
  init: function(){
    this.config();
    this.comportamiento();
    this.resize();
    return;
  },
  config: function(){
    this.body = body;
    this.headerListado = this.body.find('#headerListado');
    return;
  },
  comportamiento: function(){

    var container = this.headerListado.find('.container');
    var col01 = container.find('.col01');
    var col03 = container.find('.col03');
    var anchoBody = this.body.width();
    var anchoContainer = container.innerWidth();

    var calculo = (anchoBody - anchoContainer) / 2;
    var calculo01 = (anchoContainer * 0.25) + calculo;
    var calculo03 = calculo + 130;

    col01.css('max-width', calculo01 + 'px');
    col03.css('max-width', calculo03 + 'px');

    return;
  },
  resize: function(){

    $(window).resize(()=>{
      this.comportamiento();
    });

    return;
  }
};*/

var closeSidebar = {
  init: function(){
    this.config();
    this.cerrar();
    return;
  },
  config: function(){
    this.body = body;
    return;
  },
  cerrar: function(){
    var localBody = this.body;
    var sidebars = localBody.find('.pmd-sidebar');
    var cerrar = sidebars.find('.header .cerrar');

    cerrar.on('click', function(){

      var item = $(this);
      var sidebar = item.parents('.pmd-sidebar');
      var id = sidebar.attr('id');
      var toggle = localBody.find('[data-target="' + id + '"]').first();

      if (toggle.length > 0) toggle.trigger('click');

    });

    return;
  }
};

var masOpciones = {
  init: function () {
    this.config();
    this.comportamiento();
    return;
  },
  config: function () {
    this.body = body;
    return;
  },
  comportamiento: function () {

    var divMasOpciones = this.body.find('.mas-opciones');
    var toggle = divMasOpciones.children('.toggle');
    var cerrar = divMasOpciones.children('.opciones').children('.cerrar');

    toggle.on('click', function (e) {

      var item = $(this);
      var padre = item.parent();

      if (padre.hasClass('open')) {
        padre.removeClass('open');
      } else {
        divMasOpciones.removeClass('open');
        padre.addClass('open');
      }

    });

    cerrar.on('click', function (e) {
      var item = $(this);
      var padre = item.parents('.mas-opciones');
      padre.removeClass('open');
    });
    /*
    this.body.bind('click', function(e){
      if (!$.contains(divMasOpciones.get(0), e.target)){
        if (e.target !== divMasOpciones.get(0)) divMasOpciones.removeClass('open');
      }
    });*/

    return;
  }
};

var sacarPrimerasLetrasNombre = {
  init: function (numLetras, nombre) {
    var resul = this.sacar(numLetras, nombre);
    return resul;
  },
  sacar: function (numLetras, nombre) {
    var resul = "";
    if (nombre == undefined) return;
    var partes = nombre.split(' ');
    $.each(partes, function (c, v) {
      if (c > numLetras - 1) return false;
      var primera = v.substring(0, 1);
      resul = resul + primera;
    });

    return this.sustituirAcentos(resul);
  },
  sustituirAcentos: function (text) {
    var acentos = "ÃÀÁÄÂÈÉËÊÌÍÏÎÒÓÖÔÙÚÜÛãàáäâèéëêìíïîòóöôùúüûÑñÇç";
    var original = "AAAAAEEEEIIIIOOOOUUUUaaaaaeeeeiiiioooouuuunncc";
    for (var i = 0; i < acentos.length; i++) {
      text = text.replace(acentos.charAt(i), original.charAt(i));
    }
    return text;
  }
};

var obtenerClaseBackgroundColor = {
  init: function (nombre) {
    var resul = this.obtener(nombre);
    return resul;
  },
  obtener: function (nombre) {
    //var number = Math.floor(Math.random() * maximo) + 1;
    if (nombre == undefined) return;
    var letra = sacarPrimerasLetrasNombre.init(1, this.sustituirAcentos(nombre)).toLowerCase();
    return 'color-' + letra;
  },
  sustituirAcentos: function (text) {
    if (text == null) return;
    var acentos = "ÃÀÁÄÂÈÉËÊÌÍÏÎÒÓÖÔÙÚÜÛãàáäâèéëêìíïîòóöôùúüûÑñÇç";
    var original = "AAAAAEEEEIIIIOOOOUUUUaaaaaeeeeiiiioooouuuunncc";
    for (var i = 0; i < acentos.length; i++) {
      text = text.replace(acentos.charAt(i), original.charAt(i));
    }
    return text;
  }
};

var circulosPersona = {
  init: function () {
    this.config();
    this.circulos();
    return;
  },
  config: function () {
    this.body = body;
    this.headerResource = this.body.find('.header-resource');
    return;
  },
  circulos: function () {

      var h1Container = this.headerResource.find('.h1-container');

      var titulo = h1Container.find('h1');

      if (titulo.text() != undefined) {
        var iniciales = sacarPrimerasLetrasNombre.init(2, titulo.text());
        var clase = obtenerClaseBackgroundColor.init(titulo.text());
        var spanCirculo = $('<span />').addClass('circuloPersona ' + clase).text(iniciales);

        h1Container.append(spanCirculo);
      }

    return;
  }
};

var filtrarMovil = {
  init: function(){
    this.config();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.filtrarMovil = this.body.find('.btn-filtrar-movil');
    this.colFacetas = this.body.find('.col-facetas');
    return;
  },
  comportamiento: function(){

    var localBody = this.body;
    this.filtrarMovil.off('click').on('click', function(e){
      if (localBody.hasClass('facetas-abiertas')){
        localBody.removeClass('facetas-abiertas');
      }else{
        localBody.addClass('facetas-abiertas');
      }
    });

    this.colFacetas.find('.cerrar').off('click').on('click', function(e){
      localBody.removeClass('facetas-abiertas');
    });

    return;
  }
};

var metabuscador = {
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

    var menuLateral = this.body.find('#menuLateralMetabuscador');
    var input = menuLateral.find('#txtBusquedaPrincipal');
    var btn = this.header.find('.buscar .pmd-sidebar-toggle');

    btn.on('click', function(e){
      input.focus();
    });

    input.on('keydown', function(){
      setTimeout(function(){
        var val = input.val();
        if (val.length > 0){
          menuLateral.addClass('buscando');
        }else{
          menuLateral.removeClass('buscando');
        }
      }, 100);
    });

    return;
  }
};

var buscadorSeccion = {
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

    var headerListado = this.body.find('#headerListado');
    var input = headerListado.find('#finderSection');

    input.focusin(function(){
      headerListado.addClass('sugerencias');
    }).focusout(function(){
      headerListado.removeClass('sugerencias buscando');
    });

    input.on('keydown', function(e){
      setTimeout(function(){
        var val = input.val();
        if (val.length > 0){
          headerListado.removeClass('sugerencias').addClass('buscando');
        }else{
          headerListado.removeClass('buscando').addClass('sugerencias');
        }
      }, 100);
    });

    return;
  }
};

var listadoMensajes = {
  init: function(){
    this.config();
    this.comprobarSeleccionados();
    this.seleccionar();
    return;
  },
  config: function(){
    this.body = body;
    this.resourceList = this.body.find('.wrapCol .resource-list');
    return;
  },
  seleccionar: function(){

    var that = this;
    var resources = this.resourceList.find('.resource');

    resources.off('click').on('click', '.img-usuario a', function(e){

      e.preventDefault();
      var resource = $(this).parents('.resource');
      if (resource.hasClass('seleccionado')){
        resource.removeClass('seleccionado');
      }else{
        resource.addClass('seleccionado');
      }

      that.comprobarSeleccionados();

    });

    return;
  },
  comprobarSeleccionados: function(){

    var seleccionados = this.resourceList.find('.resource.seleccionado');
    if (seleccionados.length){
      this.body.addClass('haySeleccionados');
    }else{
      this.body.removeClass('haySeleccionados');
    }

    return;
  }
};

var cambioVistaListado = {
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

    var accionesListado = this.body.find('.wrapCol > .acciones-listado');
    var visualizacion = accionesListado.find('.visualizacion');
    var resourceList = this.body.find('.wrapCol > .resource-list');
    var lis = visualizacion.find('li');
    var a = lis.children('a');

    a.on('click', function(e){

      e.preventDefault();
      var item = $(this);
      var li = item.parent();
      var clase = li.data('class-resource-list');

      lis.removeClass('activeView');
      visualizacion.find('li[data-class-resource-list="' + clase + '"]').addClass('activeView');

      if (clase != ""){
        resourceList.removeClass('compacView listView');
        resourceList.addClass(clase);
      }
    });

    return;
  }
};

var masHeaderMensaje = {
  init: function(){
    this.config();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.headerMensaje = this.body.find('.header-mensaje');
    return;
  },
  comportamiento: function(){

    this.headerMensaje.off('click', '.ver-mas').on('click', '.ver-mas', function(e){
      e.preventDefault();
      var verMas = $(this);
      var padre = verMas.parent();
      var ul = padre.find('ul');

      ul.children().each(function(i){
        var li = $(this);
        if (padre.hasClass('abierto') && i > 1){
          li.addClass('oculto');
          padre.removeClass('abierto');
          verMas.text('más');
        }else if (!padre.hasClass('abierto') && i > 1){
          li.removeClass('oculto');
          padre.addClass('abierto');
          verMas.text('menos');
        }
      });

    });

    return;
  }
};

var autocompletar = {
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

    var input = this.body.find('#autocomplete');
    var autocomplete = this.body.find('.ui-autocomplete');

    input.keyup(function(){
      autocomplete.show();
    });

    return;
  }
}

function comportamientoCargaFacetasComunidad() {

};

$.fn.reverse = [].reverse;

var body;

$(function () {
  body = $('body');

  bodyScrolling.init();
  closeSidebar.init();
  masOpciones.init();
  metabuscador.init();

  if (window.location.href.indexOf("plantillas") > -1){
    autocompletar.init();
  }

  if(body.hasClass('fichaPersona')){
    circulosPersona.init();
  }else if (body.hasClass('listado')){
    filtrarMovil.init();
    buscadorSeccion.init();
    cambioVistaListado.init();

    if (body.hasClass('mensajes')){
      listadoMensajes.init();
    }
    if (body.hasClass('fichaMensaje')){
      masHeaderMensaje.init();
    }
  }

});











/**
 * --------------------------------------------------------------------------
 * Propeller v1.3.1 (http://propeller.in): sidebar.js
 * Copyright 2016-2018 Digicorp, Inc.
 * Licensed under MIT (http://propeller.in/LICENSE)
 * --------------------------------------------------------------------------
*/

var pmdSidebar = function ($) {


  /**
   * ------------------------------------------------------------------------
   * Variables
   * ------------------------------------------------------------------------
  */

  var NAME = 'pmdSidebar';
  var JQUERY_NO_CONFLICT = $.fn[NAME];
  var isOpenWidth = 1200;

  var ClassName = {
    OPEN: 'pmd-sidebar-open',
    SLIDE_PUSH: 'pmd-sidebar-slide-push',
    RIGHT_FIXED: 'pmd-sidebar-right-fixed',
    LEFT_FIXED: 'pmd-sidebar-left-fixed',
    OVERLAY_ACTIVE: 'pmd-sidebar-overlay-active',
    BODY_OPEN: 'pmd-body-open',
    RIGHT: 'pmd-sidebar-right',
    NAVBAR_SIDEBAR: 'pmd-navbar-sidebar',
    LEFT: 'pmd-sidebar-left',
    PM_INI: ".pm-ini",
    IS_SLIDEPUSH: "is-slidepush"
  };

  var Selector = {
    BODY: 'body',
    PARENT_SELECTOR: '',
    OVERLAY: '.pmd-sidebar-overlay',
    SIDEBAR: '.pmd-sidebar',
    LEFT: '.' + ClassName.LEFT,
    RIGHT_FIXED: '.' + ClassName.RIGHT_FIXED,
    NAVBAR_SIDEBAR: '.' + ClassName.NAVBAR_SIDEBAR,
    SIDEBAR_HEADER: '#sidebar .sidebar-header',
    TOGGLE: '.pmd-sidebar-toggle',
    TOPBAR_FIXED: '.topbar-fixed',
    SIDEBAR_DROPDOWN: '.pmd-sidebar-nav .dropdown-menu',
    TOGGLE_RIGHT: '.pmd-sidebar-toggle-right',
    TOPBAR_TOGGLE: '.pmd-topbar-toggle',
    TOPBAR_CLOSE: '.topbar-close',
    NAVBAR_TOGGLE: '.pmd-navbar-toggle',
    PM_INI: ".pm-ini",
    IS_SLIDEPUSH: '.' + ClassName.IS_SLIDEPUSH
  };

  var Event = {
    CLICK: 'click'
  };


  /**
 * ------------------------------------------------------------------------
 * Functions
 * ------------------------------------------------------------------------
 */

  // Left sidebar toggle
  function onSidebarToggle(e) {
    var dataTarget = "#" + $(e.currentTarget).attr("data-target");
    $(dataTarget).toggleClass(ClassName.OPEN);

    var allSidebars = $(Selector.SIDEBAR);

    if (($(dataTarget).hasClass(ClassName.LEFT_FIXED) || $(dataTarget).hasClass(ClassName.RIGHT_FIXED)) && $(dataTarget).hasClass(ClassName.OPEN)) {
        $(Selector.OVERLAY).addClass(ClassName.OVERLAY_ACTIVE);
        //$(Selector.OVERLAY + '[data-rel="' + $(e.currentTarget).attr("data-target") + '"]').addClass(ClassName.OVERLAY_ACTIVE);
        $(Selector.BODY).addClass(ClassName.BODY_OPEN);
    } else {
      if (!allSidebars.hasClass(ClassName.OPEN)){
        $(Selector.OVERLAY).removeClass(ClassName.OVERLAY_ACTIVE);
        //$(Selector.OVERLAY + '[data-rel="' + $(e.currentTarget).attr("data-target") + '"]').removeClass(ClassName.OVERLAY_ACTIVE);
        $(Selector.BODY).removeClass(ClassName.BODY_OPEN);
      }
    }

  }

  // Nave bar in Sidebar
  function onNavBarToggle() {
    $(Selector.NAVBAR_SIDEBAR).toggleClass(ClassName.OPEN);
    if (($(Selector.NAVBAR_SIDEBAR).hasClass(ClassName.NAVBAR_SIDEBAR)) && $(Selector.NAVBAR_SIDEBAR).hasClass(ClassName.OPEN)) {
      $(Selector.OVERLAY + '[data-rel="' + $(e.currentTarget).attr("data-target") + '"]').addClass(ClassName.OVERLAY_ACTIVE);
      $(Selector.BODY).addClass(ClassName.BODY_OPEN);
    } else {
      $(Selector.OVERLAY + '[data-rel="' + $(e.currentTarget).attr("data-target") + '"]').removeClass(ClassName.OVERLAY_ACTIVE);
      $(Selector.BODY).addClass(ClassName.BODY_OPEN);
    }
  }

  // Overlay
  function onOverlayClick(event) {
    var $this = $(event.currentTarget);
    //var rel = $this.data('rel');
    $this.removeClass(ClassName.OVERLAY_ACTIVE);
    $(Selector.SIDEBAR).removeClass(ClassName.OPEN);
    //$(Selector.SIDEBAR + "#" + rel).removeClass(ClassName.OPEN);
    $(Selector.NAVBAR_SIDEBAR).removeClass(ClassName.OPEN);
    $(Selector.BODY).removeClass(ClassName.BODY_OPEN);

    event.stopPropagation();
  }

  // On Window Resize
  function onResizeWindow(e) {
    var options = e.data.param1;
    var sideBarSelector = Selector.SIDEBAR;
    $(sideBarSelector).each(function () {
      var $this = $(this);
      var sideBarId = $this.attr("id");
      var isOpenWidth = $("a[data-target=" + sideBarId + "]").attr("is-open-width");
      if ($(window).width() < isOpenWidth) {
        if ($("#" + sideBarId).hasClass(ClassName.LEFT && ClassName.SLIDE_PUSH)) {
          $("#" + sideBarId).removeClass(ClassName.OPEN + ' ' + ClassName.SLIDE_PUSH);
          $("#" + sideBarId).addClass(ClassName.LEFT_FIXED + ' ' + ClassName.IS_SLIDEPUSH);
        } else {
          $("#" + sideBarId).removeClass(ClassName.OPEN);
        }
      } else {
        if ($("#" + sideBarId).hasClass(ClassName.IS_SLIDEPUSH)) {
          $("#" + sideBarId).addClass(ClassName.OPEN + ' ' + ClassName.SLIDE_PUSH);
          $("#" + sideBarId).removeClass(ClassName.LEFT_FIXED);
        } else {
          $("#" + sideBarId).addClass(ClassName.OPEN);
        }
      }
    });
    $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.OVERLAY + '[data-rel="' + options.init + '"]')).removeClass(ClassName.OVERLAY_ACTIVE);
    $(Selector.BODY).removeClass(ClassName.BODY_OPEN);
  }

  /**
 * ------------------------------------------------------------------------
 * Initialization
 * ------------------------------------------------------------------------
 */

  var pmdSidebar = function () {
    _inherits(pmdSidebar, commons);
    function pmdSidebar(options) {
      var sideBarSelector = Selector.TOGGLE;
      if (Selector.PARENT_SELECTOR !== "" && Selector.PARENT_SELECTOR !== undefined) {
        sideBarSelector = Selector.TOGGLE + "[data-target=" + Selector.PARENT_SELECTOR.substr(1, Selector.PARENT_SELECTOR.length) + "]";
      }
      $(sideBarSelector).each(function () {
        var $this = $(this);
        var dataTarget = "#" + $this.attr("data-target");
        var dataPlacement = $this.attr("data-placement");
        var dataPosition = $this.attr("data-position");
        var isopen = $this.attr("is-open");
        var minsize = $this.attr("minsize");
        dataPlacement = dataPlacement || "";
        dataPosition = dataPosition || "";
        if ($(sideBarSelector).attr("data-target") === undefined) {
          console.warn("You need to define 'data-target' attribute in the action button.");
        }
        if ($(Selector.SIDEBAR).attr("id") === undefined) {
          console.warn("You need to add id='" + $this.attr("data-target") + "'in the sidebar container div like <aside id='" + $this.attr("data-target") + "'class='pmd-sidebar'>");
        }
        if (dataPlacement.toLowerCase() === "left") {
          $(dataTarget).addClass(ClassName.LEFT);
        } else if (dataPlacement.toLowerCase() === "right") {
          $(dataTarget).addClass(ClassName.RIGHT_FIXED);
        } else {
          $(dataTarget).addClass(ClassName.LEFT);
        }
        if (dataPlacement.toLowerCase() === "left" && dataPosition.toLowerCase() === "slidepush") {
          $(dataTarget).addClass(ClassName.SLIDE_PUSH);
        } else if (dataPlacement.toLowerCase() === "left" && dataPosition.toLowerCase() === "fixed") {
          $(dataTarget).addClass(ClassName.LEFT_FIXED);
        } else if (dataPlacement.toLowerCase() === "right" && dataPosition.toLowerCase() === "slidepush") {

        } else if (dataPlacement.toLowerCase() === "right" && dataPosition.toLowerCase() === "fixed") {
          $(dataTarget).addClass(ClassName.RIGHT_FIXED);
        } else {
          $(dataTarget).addClass(ClassName.LEFT_FIXED);
        }
        if (isopen !== undefined && isopen !== null && (isopen === true || isopen === "true")) {
          $(dataTarget).addClass(ClassName.OPEN);
        } else {
          $(dataTarget).removeClass(ClassName.OPEN);
        }
        $(dataTarget + ' ' + Selector.SIDEBAR_DROPDOWN).off();
        $(dataTarget + ' ' + Selector.SIDEBAR_DROPDOWN).on(Event.CLICK, function (event) {
          event.stopPropagation();
        });
        $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.TOPBAR_TOGGLE)).off(Event.CLICK);
        $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.TOPBAR_TOGGLE)).on(Event.CLICK, function (e) { $(Selector.TOPBAR_FIXED).toggleClass(ClassName.OPEN); });
        $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.TOPBAR_CLOSE)).off(Event.CLICK);
        $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.TOPBAR_CLOSE)).on(Event.CLICK, function () { $(Selector.TOPBAR_FIXED).removeClass(ClassName.OPEN); });
        $this.off(Event.CLICK);
        $this.on(Event.CLICK, onSidebarToggle);
        var isOpenWidth = $this.attr("is-open-width");
        if ($(window).width() < isOpenWidth) {
          if ($(dataTarget).hasClass(ClassName.LEFT && ClassName.SLIDE_PUSH)) {
            $(dataTarget).removeClass(ClassName.OPEN + ' ' + ClassName.SLIDE_PUSH);
            $(dataTarget).addClass(ClassName.LEFT_FIXED + ' ' + ClassName.IS_SLIDEPUSH);
          } else {
            $(dataTarget).removeClass(ClassName.OPEN);
          }
        } else {
          if ($(dataTarget).hasClass(ClassName.IS_SLIDEPUSH)) {
            $(dataTarget).addClass(ClassName.OPEN + ' ' + ClassName.SLIDE_PUSH);
            $(dataTarget).removeClass(ClassName.LEFT_FIXED);
          } else {
            //			$(dataTarget).addClass(ClassName.OPEN);
          }
        }

      });

      $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.NAVBAR_TOGGLE)).off(Event.CLICK);
      $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.NAVBAR_TOGGLE)).on(Event.CLICK, onNavBarToggle);
      $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.OVERLAY)).off(Event.CLICK);
      $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.OVERLAY)).on(Event.CLICK, onOverlayClick);

      //	$(window).unbind("resize");
      //$(window).resize({ param1: options }, onResizeWindow);
      (function (removeClass) {
        jQuery.fn.removeClass = function (value) {
          if (value && typeof value.test === "function") {
            for (var i = 0, l = this.length; i < l; i++) {
              var elem = this[i];
              if (elem.nodeType === 1 && elem.className) {
                var classNames = elem.className.split(/\s+/);

                for (var n = classNames.length; n--;) {
                  if (value.test(classNames[n])) {
                    classNames.splice(n, 1);
                  }
                }
                elem.className = jQuery.trim(classNames.join(" "));
              }
            }
          } else {
            removeClass.call(this, value);
          }
          return this;
        };
      })(jQuery.fn.removeClass);
    }
    return pmdSidebar;
  }();


  /**
   * ------------------------------------------------------------------------
   * jQuery
   * ------------------------------------------------------------------------
  */

  var plugInFunction = function (arg) {
    if (this.selector !== "") {
      Selector.PARENT_SELECTOR = this.selector;
    }
    new pmdSidebar(arg);
  };
  $.fn[NAME] = plugInFunction;
  return pmdSidebar;

}(jQuery)();