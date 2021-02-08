/*!
 * jQuery JavaScript Library v1.6.1
 * http://jquery.com/
 *
 * Copyright 2011, John Resig
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * Includes Sizzle.js
 * http://sizzlejs.com/
 * Copyright 2011, The Dojo Foundation
 * Released under the MIT, BSD, and GPL Licenses.
 *
 * Date: Thu May 12 15:04:36 2011 -0400
 */

var configWidgetGNOSS = {
  cssWidget: '.widget',
  cssBodyWidget: '.widget-buscador-body-gnoss',
  cssFooterWidget: '.widget-buscador-foot-gnoss',
  cssWidgetConfig: '.widgetConfig',
  cssAncho: '.cAncho',
  cssAnchoMax: '.cAnchoMax',
  cssAnchoMin: '.cAnchoMin',
  cssBtProbar: '.btProbar',
  cssBtGenerar: '.btGenerar',
  cssUrlWidget: '.urlWidget',
  idCodigo: '#widget-codigo-gnoss',
  init: function () {
    this.config();
    this.actualizarCodigo();
    this.engancharProbar();
    return;
  },
  config: function () {
    this.widget = $('#section ' + this.cssWidget);
    this.widgetConfig = $('#section ' + this.cssWidgetConfig);
    this.inputAncho = $(this.cssAncho, this.widgetConfig);
    this.anchoMaximo = $('#section ' + this.cssAnchoMax).html();
    this.anchoMinimo = $('#section ' + this.cssAnchoMin).html();
    this.btProbar = $(this.cssBtProbar, this.widgetConfig);
    this.btGenerar = $(this.cssBtGenerar, this.widgetConfig);
    this.btAjustar = $(this.cssBtAjustar, this.widgetConfig);
    this.codigo = $(this.idCodigo);
    this.urlWidget = $(this.cssUrlWidget);
    return;
  },
  comprobarAncho: function () {
    var ancho = parseInt(this.inputAncho.val());
    if (ancho > this.anchoMaximo) {
      ancho = this.anchoMaximo;
    } else if (ancho < this.anchoMinimo) {
      ancho = this.anchoMinimo;
    };
    return ancho + 'px';
  },
  codigoMostrar: function (ancho, background, urlComunidad, urlStatic, nombreComunidad, urlLogo, pagina, urlJS, versionGnossJS) {//
    if (typeof urlJS == "undefined" || urlJS == "") {
      urlJS = urlStatic + '/jsNuevo';
    }
    else {
      urlStatic = urlJS;
    }
    var html = '';
    html += '<script type="text/javascript" src="' + urlJS + '/jquery.gnoss.widget.buscador.js' + versionGnossJS + '"></script>' + '\n';
    html += '<script>' + '\n';
    html += 'new widgetGnoss.widget({' + '\n';
    if (ancho != "") {
      html += '\t' + 'ancho: \'' + ancho + '\', ' + '\n';
    }
    if (background != "") {
      html += '\t' + 'background: \'' + background + '\', ' + '\n';
    }
    html += '\t' + 'urlComunidad: \'' + urlComunidad + '\', ' + '\n';
    html += '\t' + 'pagina: \'' + pagina + '\', ' + '\n';
    html += '\t' + 'urlStatic: \'' + urlStatic + '\', ' + '\n';
    html += '\t' + 'nombreComunidad: \'' + nombreComunidad + '\', ' + '\n';
    html += '\t' + 'urlLogo: \'' + urlLogo + '\'' + '\n';
    html += '});' + '\n';
    html += '</script>';

    return html;
  },
  actualizarCodigo: function () {
    var ancho = $(".cRadioAncho input[type=radio]:checked").val() == 'auto' ? '' : this.comprobarAncho();

    var background = ""
    if ($(".defaultBackground").length > 0) {
      background = $(".defaultBackground input[type=radio]:checked").val() == 'defaultColor' ? '#D9DBF7' : '#ffffff';
    }

    var urlComunidad = $(".cUrlComunidad").val();
    var urlStatic = $(".cUrlStatic").val();
    var urlJS = $(".cUrlJS").val();

    urlStatic = $('#inpt_baseURLContent').val() + "/imagenes/proyectos/personalizacion/" + $('#inpt_proyID').val() + "/theme";
    urlJS = $('#inpt_baseURLContent').val() + "/imagenes/proyectos/personalizacion/" + $('#inpt_proyID').val() + "/theme";

    var nombreComunidad = $(".cNombreComunidad").val();
    var urlLogo = $(".cUrlLogo").val();
    var pagina = '';
    if ($(".cPagina").length > 0) { pagina = $(".cPagina").val(); }
    var versionGnossJS = $(".cVersionJs").val();

    var codigoMostrar = this.codigoMostrar(ancho, background, urlComunidad, urlStatic, nombreComunidad, urlLogo, pagina, urlJS, versionGnossJS);
    this.codigo.find('textarea').val(codigoMostrar);

    if (typeof urlJS == "undefined" || urlJS == "") {
      urlJS = urlStatic + '/jsNuevo';
    }
    else {
      urlStatic = urlJS;
    }
    var codigo = widgetGnoss.codigoWidget(ancho, background, urlComunidad, urlStatic, nombreComunidad, urlLogo, pagina, urlJS);
    $("#widgetBuscador").html(codigo);
  },
  engancharProbar: function () {
    var that = this;
    this.btProbar.bind('click', function (evento) {
      evento.preventDefault();
      that.codigo.hide();
      that.actualizarCodigo();
    });
  }
}
generarCodigoGNOSS = {
  btGenerar: '.btGenerar',
  idWidget: '#widget-codigo-gnoss',
  init: function () {
    this.config();
    var that = this;
    $(this.btGenerar).each(function () {
      $(this).bind('click', function (evento) {
        //evento.preventDefault();
        that.widget.show();
      });
    })
  },
  config: function () {
    this.widget = $(this.idWidget);
    return;
  },
  enganchar: function () {
    return;
  }
}
$(function () {
  configWidgetGNOSS.init();
  generarCodigoGNOSS.init();
});