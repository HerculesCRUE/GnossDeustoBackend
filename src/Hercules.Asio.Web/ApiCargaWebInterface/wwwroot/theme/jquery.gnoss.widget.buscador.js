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

var widgetGnoss = {
    widget: function (object) {
        var codigo = widgetGnoss.codigoWidget(object.ancho, object.background, object.urlComunidad, object.urlStatic, object.nombreComunidad, object.pagina);
        document.write(codigo);
    },
    focus: function (input) {
        this.defaultInput = input;
        this.defaultInputText = this.defaultInput.attributes['value'].value;
        var texto = this.defaultInput.value;
        if (texto == '' || texto == this.defaultInputText) {
            this.defaultInput.value = '';
            this.defaultInput.className = this.defaultInput.className.replace('defaultText', '').trim()
        }
    },
    blur: function (input) {
        this.defaultInput = input;
        this.defaultInputText = this.defaultInput.attributes['value'].value;
        var texto = this.defaultInput.value;
        if (texto == '' || texto == this.defaultInputText) {
            this.defaultInput.className = this.defaultInput.className.trim() + ' defaultText';
            this.defaultInput.value = this.defaultInputText;
        }
    },
    comprobarTecla: function (input, urlComunidad, pagina, event) {
        this.defaultInput = input;
        if ((event.which == 13) || (event.keyCode == 13)) {
            this.enviarFrom(input, urlComunidad, pagina); 
        }
    },
    enviarFrom: function (input, urlComunidad, pagina) {
        this.defaultInput = input;
        this.defaultInputText = this.defaultInput.attributes['value'].value;
        var texto = this.defaultInput.value;
        if (texto == this.defaultInputText)
        {
            texto = '';
        }
        var url = urlComunidad + '/' + pagina + '?search=';
        if (texto == '') { url = url.replace('?search=', ''); };
        window.open(url + texto);
    },
    codigoWidget: function (ancho, background, urlComunidad, urlStatic, nombreComunidad, urlLogo, pagina, urlJS) {
        var html = '';
		html += '<link href="' + urlStatic + '/widgets.css" rel="stylesheet" type="text/css" media="screen">';
		
		if(pagina == null || pagina == "" ){
			pagina = 'recursos';
		}

        var estilo = "";
        if ((ancho != null && ancho != "") || (background != null && background != "")) {
            estilo = "style=\"";
            if (ancho != null && ancho != "") { estilo += "width: " + ancho + ";" }
            estilo += "\"";
        }

        html += '<div class="-wbg" ' + estilo + '>';
        html += '<div class="-wbg-section">';
		html += '<img class="-wbg-logo" src="' + urlStatic + '/resources/logo-educere-negro.png" />';
		html += '<p class="-wbg-encontrar">Encontrar recursos en Educere</p>';
        var textoMarcaAgua = 'Busca recursos educativos';
        html += '<div class="-wbg-finder">';
        html += '<input type="text" onkeydown="widgetGnoss.comprobarTecla(this, \'' + urlComunidad + '\', \'' + pagina + '\', event)" autocomplete="off" id="gnoss_txtBusqueda" class="defaultText" value="' + textoMarcaAgua + '" onfocus="widgetGnoss.focus(this);" onblur="widgetGnoss.blur(this);">';
        html += '<input type="button" id="gnoss_btnBuscar" value="Buscar" onclick="widgetGnoss.enviarFrom(this.previousSibling, \'' + urlComunidad + '\', \'' + pagina + '\')">';
        html += '<\/div>';
        html += '<\/div>';
        html += '<p class="-wbg-author">Powered by <a href="' + urlComunidad + '"><img src="' + urlStatic + '/resources/logo_gnoss_blanco.png" title="' + nombreComunidad + '" alt="' + nombreComunidad + '"></a><\/p>';        
        html += '<\/div>';
        return html;
    }
}