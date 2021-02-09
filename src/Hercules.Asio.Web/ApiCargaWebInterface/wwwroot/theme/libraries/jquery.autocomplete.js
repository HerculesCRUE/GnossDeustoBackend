/*
 * jQuery Autocomplete plugin 1.1
 *
 * Copyright (c) 2009 JÃ¶rn Zaefferer
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * Revision: $Id: jquery.autocomplete.js 15 2009-08-22 10:30:27Z joern.zaefferer $
 */

; (function($) {
	
$.fn.extend({
	autocomplete: function(urlOrData, options) {
		var isUrl = typeof urlOrData == "string";
		options = $.extend({}, $.Autocompleter.defaults, {
			url: isUrl ? urlOrData : null,
			data: isUrl ? null : urlOrData,
			delay: isUrl ? $.Autocompleter.defaults.delay : 10,
			max: options && !options.scroll ? 10 : 150,
            urlmultiple: null,
            urlActual: 0,
            urlParteAsmx: null,
            urlServicio: function() {
					if (this.urlmultiple != null)
                    {
                        var urlServ = this.urlmultiple[this.urlActual] + this.urlParteAsmx;
                        this.urlActual++;
                        if (this.urlActual == this.urlmultiple.length){this.urlActual = 0}
                        return urlServ;
                    }
					if (this.servicio != null) {
					    return this.servicio.service;
					}
					else {
					    return this.url;
					}
				}
		}, options);
		
		// if highlight is set to false, replace it with a do-nothing function
		options.highlight = options.highlight || function(value) { return value; };
		
		// if the formatMatch option is not specified, then use formatItem for backwards compatibility
		options.formatMatch = options.formatMatch || options.formatItem;

        //Cargo las urls multiples en caso de haberlas:
        ObtenerUrlMultiple(options);
		
		return this.each(function() {
			new $.Autocompleter(this, options);
		});
	},
	result: function(handler) {
		return this.bind("result", handler);
	},
	search: function(handler) {
		return this.trigger("search", [handler]);
	},
	flushCache: function() {
		return this.trigger("flushCache");
	},
	setOptions: function(options){
		return this.trigger("setOptions", [options]);
	},
	unautocomplete: function() {
		return this.trigger("unautocomplete");
	}
});

$.Autocompleter = function(input, options) {

	var KEY = {
        LEFT: 37,
		UP: 38,
        RIGHT: 39,
		DOWN: 40,
		DEL: 46,
		TAB: 9,
		RETURN: 13,
		ESC: 27,
		COMMA: 188,
		PAGEUP: 33,
		PAGEDOWN: 34,
		BACKSPACE: 8
	};

	// Create $ object for input element
	var $input = $(input).attr("autocomplete", "off").addClass(options.inputClass);

    var cont = 0;
	var timeout;
	var previousValue = "";
	var cache = $.Autocompleter.Cache(options);
	var hasFocus = 0;
	var lastKeyPressCode;
	var config = {
		mouseDownOnSelect: false
	};
	var select = $.Autocompleter.Select(options, input, selectCurrent, pintarSeleccionado, config);
	
	var blockSubmit;
	
	// prevent form submit in opera when selecting with return key
//	$.browser.opera && $(input.form).bind("submit.autocomplete", function() {
//		if (blockSubmit) {
//			blockSubmit = false;
//			return false;
//		}
//	});
	
	 // only opera doesn't trigger keydown multiple times while pressed, others don't work with keypress at all
	$input.bind((/*$.browser.opera ? "keypress" : */"keyup") + ".autocomplete", function(event) {
		// a keypress means the input has focus
		// avoids issue where input had focus before the autocomplete was applied
		hasFocus = 1;
		// track last key pressed
		lastKeyPressCode = event.keyCode;
		switch(event.keyCode) {
		
			case KEY.UP:
				event.preventDefault();
				if ( select.visible() ) {
					select.prev();
				} else {
					onChange(0, true);
				}
				break;
				
			case KEY.DOWN:
				event.preventDefault();
				if ( select.visible() ) {
					select.next();
				} else {
					onChange(0, true);
				}
				break;
				
			case KEY.PAGEUP:
				event.preventDefault();
				if ( select.visible() ) {
					select.pageUp();
				} else {
					onChange(0, true);
				}
				break;
				
			case KEY.PAGEDOWN:
				event.preventDefault();
				if ( select.visible() ) {
					select.pageDown();
				} else {
					onChange(0, true);
				}
				break;
			// matches also semicolon
			case options.multiple && $.trim(options.multipleSeparator) == "," && KEY.COMMA:
				select.hide();
				PintarTags($input);
				break;
			case KEY.TAB:
			case KEY.RETURN:
			    cancelEvent(event);
				if( selectCurrent() ) {
					// stop default to prevent a form submit, Opera needs special handling
					event.preventDefault();
					blockSubmit = true;
					return false;
				}
				select.hide();
				PintarTags($input);
				break;
			case KEY.LEFT:
			case KEY.RIGHT:
			case KEY.ESC:
				select.hide();
				break;
			default:
				clearTimeout(timeout);
				timeout = setTimeout(onChange, options.delay);
				break;
		}
	}).focus(function(){
		// track whether the field has focus, we shouldn't process any
		// results if the field no longer has focus
		hasFocus++;
	}).blur(function() {
		hasFocus = 0;
		if (!config.mouseDownOnSelect) {
			hideResults();
		}
	}).click(function() {
		// show select when clicking in a focused field
		if ( hasFocus++ > 1 && !select.visible() ) {
			onChange(0, true);
		}
	}).bind("search", function() {
		// TODO why not just specifying both arguments?
		var fn = (arguments.length > 1) ? arguments[1] : null;
		function findValueCallback(q, data) {
			var result;
			if( data && data.length ) {
				for (var i=0; i < data.length; i++) {
					if( data[i].result.toLowerCase() == q.toLowerCase() ) {
						result = data[i];
						break;
					}
				}
			}
			if( typeof fn == "function" ) fn(result);
			else $input.trigger("result", result && [result.data, result.value]);
		}
		$.each(trimWords($input.val()), function(i, value) {
			request(value, findValueCallback, findValueCallback);
		});
	}).bind("flushCache", function() {
		cache.flush();
	}).bind("setOptions", function() {
		$.extend(options, arguments[1]);
		// if we've updated the data, repopulate
		if ( "data" in arguments[1] )
			cache.populate();
	}).bind("unautocomplete", function() {
		select.unbind();
		$input.unbind();
		$(input.form).unbind(".autocomplete");
	});
	
	
	function selectCurrent() {
		var selected = select.selected();
		if( !selected )
		{
            forzarClickBoton('', '');
			return false;
		}
        
        pintarSeleccionado($input, selected.result);
		
		hideResultsNow();
		$input.trigger("result", [selected.data, selected.value]);
			
        var faceta = '';
		
		if (selected.data.length > 2 /*&& selected.data[2] != ''*/)
		{
//		    var url = selected.data[2];
//		    url = url.substring(url.indexOf('url=') + 4);
//		    
//		    if (url.indexOf('|||') != -1)
//		    {
//		        url = url.substring(0, url.indexOf('|||'));
//		    }
		    
//		    if (selected.data[1] == 'foaf:firstName')
//		    {
//		        window.location.href = baseUrlBusqueda + '/' + urlRecursosBusqueda + '/' + url;
//		    }
//		    else if (selected.data[1] == 'gnoss:hasnombrecompleto')
//		    {
//		        url = url.replace('[perfil]',urlPerfilBusqueda).replace('[organizacion]',urlOrganizacionBusqueda).replace('[clase]',urlClaseBusqueda);
//		        window.location.href = baseUrlBusqueda + '/' + url;
//		    }
//          else if (selected.data[1] == 'formSem')
            if (selected.data[1] == 'formSem')
            {
                AgregarEntidadSeleccAutocompletar(selected.data);
            }
            else if (selected.data[1] == 'formSemGrafoDependiente')
            {
                AgregarValorGrafoDependienteAutocompletar(selected.data);
            }
            else if (selected.data[1] == 'formSemGrafoAutocompletar')
            {
                AgregarValorGrafoAutocompletar(selected.data);
            }
            else if (selected.data[1] == 'idioma' || selected.data[1].indexOf('[MultiIdioma]') != -1)
            {
                $input.val($input.val() + '@' + $('input.inpt_Idioma').val());
            }            
            else if (selected.data[1] == 'datoextraproyectovirtuoso')
            {
                $input.val($input.val());
                $input.attr('aux',$input.val());
                var inputHidden = $input.parent().find($('#'+$input.attr('id') + 'hack'));
                if(typeof(inputHidden.attr('id')) != 'undefined')
                {
                    inputHidden.val(selected.data[2]);
                    inputHidden.attr('aux',selected.data[2]);
                }
            }
            else if (typeof (facetasCMS) != 'undefined' && facetasCMS)
            {
                var botonBuscar = document.getElementById(options.extraParams.botonBuscar);
                if (botonBuscar.attributes['href'] != null)
                {
                    var urlRedirect = botonBuscar.attributes['href'].value;

                    if (urlRedirect.indexOf('?') != -1)
                    {
                        urlRedirect = urlRedirect.substring(0, rlRedirect.indexOf('?'));
                    }

                    window.location =  urlRedirect + '?' + selected.data[1] + '=' + $input.val();
                    return true;
                }
            }
            else
            {
                forzarClickBoton('', selected.result);
		        return true;
            }
		}
		else if (selected.data.length > 1)
		{
//		    faceta = selected.data[1];
		}
			
		forzarClickBoton(faceta, selected.result);
        
		return true;
	}
	
    function pintarSeleccionado(textbox, resultado)
    {
        if (!options.pintarConcatenadores)
        {
            resultado = QuitarContadores(resultado);
        }

        if (textbox.attr('id') == 'finderSection' || textbox.attr('id') == 'txtBusquedaPrincipal')
        {
            /*Si es el buscador de una pÃ¡gina de busqueda o el metabuscador superior, autocompleta con "" */
            resultado='"'+resultado+'"';
        }   


        var v = resultado;
        previousValue = v;
		var cursorAt = textbox.selection().start;
		if(cursorAt < 0)
		{
		    cursorAt = textbox.val().indexOf(v) + resultado.length;
		}
        
	    if ( options.multiple ) {
		    var words = trimWords(textbox.val());
		    if ( words.length > 1 ) 
		    {
			    var seperator = options.multipleSeparator.length;
			    var wordAt, progress = 0;
			    $.each(words, function(i, word) {
				    progress += word.length;
				    if (cursorAt <= progress) {
					    wordAt = i;
					    return false;
				    }
				    progress += seperator;
			    });
			    words[wordAt] = v;
			    v = words.join( options.multipleSeparator );
		    }
	    }
	    
	    if (options.NoPintarSeleccionado == null || !options.NoPintarSeleccionado)
	    {
	        textbox.val(v);
	        PintarTags(textbox);
	    }
    }
	
	function forzarClickBoton(pFaceta, result)
	{		
	    //envia los datos al seleccionar una fila del autocompletar
        var objecte = document.getElementById(options.extraParams.botonBuscar);
		if(objecte != null)
		{
		    if (pFaceta != '') {
		        if (objecte.attributes['onclick'].value.indexOf('= url + parametros') != -1) {
		            eval(objecte.attributes['onclick'].value.replace('= url + parametros', '= url.replace("search=","' + pFaceta + '=") + parametros'));
		        }
		        else {
		            eval(objecte.attributes['onclick'].value.replace('search=', pFaceta + '=').replace('return false;', ''));
		        }
		    }
		        /*else if(document.createEvent)
                {
                    var evObj = document.createEvent('MouseEvents');
                    evObj.initEvent( 'click', true, false );
                    objecte.dispatchEvent(evObj);
                }*/
		    else {
		        if ($input.val().indexOf("\"") != 0 && $input.val().lastIndexOf("\"") != $input.val().length - 1 && typeof (result) != 'undefined' && result == '' && typeof ($input[0]) != 'undefined' && typeof ($input[0].className) != 'undefined' && $input[0].className.indexOf("filtroFaceta") >= 0) {
		            pintarSeleccionado($input, '>>' + $input.val())
		        }
		        objecte.click();
		    }
        }
	}
	
	function onChange(crap, skipPrevCheck) {
		if( lastKeyPressCode == KEY.DEL ) {
			select.hide();
			return;
		}
		
		var currentValue = $input.val();
		
		if ( !skipPrevCheck && currentValue == previousValue )
			return;
		
		previousValue = currentValue;
		
		currentValue = lastWord(currentValue);
		if ( currentValue.length >= options.minChars) {
			$input.addClass(options.loadingClass);
			if (!options.matchCase)
				currentValue = currentValue.toLowerCase();
			request(currentValue, receiveData, hideResultsNow);
		} else {
			stopLoading();
			select.hide();
		}
	};
	
	function trimWords(value) {
		if (!value)
			return [""];
		if (!options.multiple)
			return [$.trim(value)];
		return $.map(value.split(options.multipleSeparator), function(word) {
			return $.trim(value).length ? $.trim(word) : null;
		});
	}
	
	function lastWord(value) {
		if ( !options.multiple )
			return value;
		var words = trimWords(value);
		if (words.length == 1) 
			return words[0];
		var cursorAt = $(input).selection().start;
		if (cursorAt == value.length) {
			words = trimWords(value)
		} else {
			words = trimWords(value.replace(value.substring(cursorAt), ""));
		}
		return words[words.length - 1];
	}
	
	// fills in the input box w/the first match (assumed to be the best match)
	// q: the term entered
	// sValue: the first matching result
	function autoFill(q, sValue){
		// autofill in the complete box w/the first match as long as the user hasn't entered in more data
		// if the last user key pressed was backspace, don't autofill
		if( options.autoFill && (lastWord($input.val()).toLowerCase() == q.toLowerCase()) && lastKeyPressCode != KEY.BACKSPACE ) {
			// fill in the value (keep the case the user has typed)
			$input.val($input.val() + sValue.substring(lastWord(previousValue).length));
			// select the portion of the value not typed by the user (so the next character will erase)
			$(input).selection(previousValue.length, previousValue.length + sValue.length);
		}
	};

	function hideResults() {
		clearTimeout(timeout);
		timeout = setTimeout(hideResultsNow, 200);
	};

	function hideResultsNow() {
		var wasVisible = select.visible();
		select.hide();
		clearTimeout(timeout);
		stopLoading();
		if (options.mustMatch) {
			// call search and run callback
			$input.search(
				function (result){
					// if no value found, clear the input box
					if( !result ) {
						if (options.multiple) {
							var words = trimWords($input.val()).slice(0, -1);
							$input.val( words.join(options.multipleSeparator) + (words.length ? options.multipleSeparator : "") );
						}
						else {
							$input.val( "" );
							$input.trigger("result", null);
						}
					}
				}
			);
		}
	};

	function receiveData(q, data) {
		if ( data && data.length && hasFocus ) {
			stopLoading();
			select.display(data, q);
			autoFill(q, data[0].value);

			if (typeof (completadaCargaAutocompletar) != "undefined") {
			    completadaCargaAutocompletar();
			}

			select.show();
		} else {
			hideResultsNow();
		}
	};

	function request(term, success, failure) {
		if (typeof(requestAutocompletarPersonalizado) != 'undefined'){
			return requestAutocompletarPersonalizado(term, success, failure, options, cache, cont, lastWord, parse, normalize);
		}
	
	    term = replaceAll(replaceAll(replaceAll(term, '%', '%25'), '#', '%23'), '+', "%2B");
		if (!options.matchCase)
			term = term.toLowerCase();
		var data = null;
		
		if (options.data == null){
			data = cache.load(term);
		}

		cont = cont + 1;
		var extraParams = {
		    q: lastWord(term),
		    limit: options.max,
		    cont: cont,
		    lista: '',
		    callback: 'autocomplete'
		};
		if (options.multiple) {
		    if (options.classTxtValoresSelecc != null) {
		        var valorLista = '';

		        $('.' + options.classTxtValoresSelecc).each(function () {
		            valorLista += $(this).val().replace(/&/g, ',');
		        });

		        extraParams["lista"] = valorLista;
		    }
		    else if (options.txtValoresSeleccID == null) {
		        extraParams["lista"] = $('#' + input.id + '_Hack').val().trim() + $input.val().trim();
		        //extraParams["lista"] = previousValue.trim();
		    }
		    else {
		        if (options.txtValoresSeleccID.indexOf('|') != -1) {
		            var idtxthacks = options.txtValoresSeleccID.split('|');
		            var valorLista = '';
		            for (var i = 0; i < idtxthacks.length; i++) {
		                valorLista += document.getElementById(idtxthacks[i]).value.replace(/&/g, ',');
		            }
		            extraParams["lista"] = valorLista;
		        }
		        else {
		            extraParams["lista"] = document.getElementById(options.txtValoresSeleccID).value.replace(/&/g, ',');
		        }
		    }
		}
		$.each(options.extraParams, function (key, param) {
		    extraParams[key] = typeof param == "function" ? param() : param;
		});

		// recieve the cached data
		if (data && data.length) {
			success(term, data);
		// if an AJAX url has been supplied, try loading the data now
		} else if ((typeof options.url == "string") && (options.url.length > 0)) {
		    var urlPost = options.urlServicio(options);

			$.ajax({
			    type: "POST",
			    url: urlPost,
			    data: extraParams,
			    cache: false
			}).done(function (response) {
			    var data = response;
			    if (response.d != null) {
			        data = response.d;
			    }

                var parsed = options.parse && options.parse(data) || parse(data);
                cache.add(term, parsed);
                success(term, parsed);
            }).fail(function (data) {

		    });
		    //$.post(options.url, extraParams, function (response) {
		    //    var parsed = options.parse && options.parse(response) || parse(response);
		    //    cache.add(term, parsed);
		    //    success(term, parsed);
		    //}, "json");
		}
		else if (options.servicio != null) {
		    

            options.servicio.service = options.urlServicio(options);

		    options.servicio.call(options.metodo, extraParams, function(data) {
	            if(extraParams.cont == cont && $('#' + extraParams.botonBuscar).prev().parent().css('display') != 'none')
	            {
				    var parsed = options.parse && options.parse(data) || parse(data);
				    cache.add(term, parsed);
				    success(term, parsed);
				}
			});	
		}
		else if (options.data != null && options.data.length > 0){
			var parsed = [];
			var termNorm = normalize(term.toLowerCase());
			
			for (var i=0;i<options.data.length;i++){
				var nombreBuscar = normalize(options.data[i][0].toLowerCase());
				if (nombreBuscar.indexOf(termNorm) == 0 || nombreBuscar.indexOf(' ' + termNorm) != -1){
					parsed.push({'data':options.data[i], 'value':options.data[i][0], 'result':options.data[i][0]});
				}
			}
		
			success(term, parsed);
		}
		else {
			// if we have a failure, we need to empty the list -- this prevents the the [TAB] key from selecting the last successful match
			select.emptyList();
			failure(term);
		}
	};
	
	
	var normalize = (function() {
	  var from = "ÃƒÃ€ÃÃ„Ã‚ÃˆÃ‰Ã‹ÃŠÃŒÃÃÃŽÃ’Ã“Ã–Ã”Ã™ÃšÃœÃ›Ã£Ã Ã¡Ã¤Ã¢Ã¨Ã©Ã«ÃªÃ¬Ã­Ã¯Ã®Ã²Ã³Ã¶Ã´Ã¹ÃºÃ¼Ã»Ã‘Ã±Ã‡Ã§", 
		  to   = "AAAAAEEEEIIIIOOOOUUUUaaaaaeeeeiiiioooouuuunncc",
		  mapping = {};
	 
	  for(var i = 0, j = from.length; i < j; i++ )
		  mapping[ from.charAt( i ) ] = to.charAt( i );
	 
	  return function( str ) {
		  var ret = [];
		  for( var i = 0, j = str.length; i < j; i++ ) {
			  var c = str.charAt( i );
			  if( mapping.hasOwnProperty( str.charAt( i ) ) )
				  ret.push( mapping[ c ] );
			  else
				  ret.push( c );
		  }      
		  return ret.join( '' );
	  }
	 
	})();
	
	function parse(data) {
		var parsed = [];
		try
		{
			var rows = data.split("\n");
			for (var i=0; i < rows.length; i++) {
				var row = $.trim(rows[i]);
				if (row) {
				    if (row.indexOf('|||') != -1)
					{
					    row = row.split("|||");
					}
					else if (row.indexOf('|') != -1)
					{
					    var valor = row.substring(0, row.lastIndexOf('|'));
					    var attControl = row.substring(row.lastIndexOf('|') + 1);
					    row = [valor, attControl];
					}
					else
					{
					    row = row.split("|");//Para que cree un array de un elemento.
					}
					
					parsed[parsed.length] = {
						data: row,
						value: row[0],
						result: options.formatResult && options.formatResult(row, row[0]) || row[0]
					};
				}
			}
		}
		catch(ex)
		{}
		return parsed;
	};

	function stopLoading() {
		$input.removeClass(options.loadingClass);
	};
};

function QuitarContadores(cadena)
{
    var resultado = cadena;

    // Obtener la cadena entre parÃ©ntesis
    var contenidoParentesis = resultado.substring(resultado.lastIndexOf('(') + 1);
    contenidoParentesis = contenidoParentesis.substring(0, contenidoParentesis.lastIndexOf(')'));

    // Si es un entero, quitamos los parÃ©ntesis
    if (contenidoParentesis == parseInt(contenidoParentesis)) {
        if (cadena.lastIndexOf('(') > -1) {
            if (cadena.lastIndexOf(')') > -1 && cadena.lastIndexOf(')') > cadena.lastIndexOf('(')) {
                resultado = cadena.substring(0, cadena.lastIndexOf('(') - 1);
            }
        }
    }

    return resultado;
}

$.Autocompleter.defaults = {
	inputClass: "ac_input",
	resultsClass: "ac_results",
	loadingClass: "ac_loading",
	minChars: 1,
	delay: 400,
	matchCase: false,
	matchSubset: true,
	matchContains: false,
	cacheLength: 10,
	max: 100,
	mustMatch: false,
	extraParams: {},
	selectFirst: true,
	formatItem: function(row) { return row[0]; },
	formatMatch: null,
	autoFill: false,
	width: 0,
	multiple: false,
	multipleSeparator: ", ",
	/*highlight: function(value, term) {
		return value.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>");
	},*/
    scroll: true,
    scrollHeight: 180
};

$.Autocompleter.Cache = function(options) {

	var data = {};
	var length = 0;
	
	function matchSubset(s, sub) {
		if (!options.matchCase) 
			s = s.toLowerCase();
		var i = s.indexOf(sub);
		if (options.matchContains == "word"){
			i = s.toLowerCase().search("\\b" + sub.toLowerCase());
		}
		if (i == -1) return false;
		return i == 0 || options.matchContains;
	};
	
	function add(q, value) {
		if (length > options.cacheLength){
			flush();
		}
		if (!data[q]){ 
			length++;
		}
		data[q] = value;
	}
	
	function populate(){
		if( !options.data ) return false;
		// track the matches
		var stMatchSets = {},
			nullData = 0;

		// no url was specified, we need to adjust the cache length to make sure it fits the local data store
		if( !options.url ) options.cacheLength = 1;
		
		// track all options for minChars = 0
		stMatchSets[""] = [];
		
		// loop through the array and create a lookup structure
		for ( var i = 0, ol = options.data.length; i < ol; i++ ) {
			var rawValue = options.data[i];
			// if rawValue is a string, make an array otherwise just reference the array
			rawValue = (typeof rawValue == "string") ? [rawValue] : rawValue;
			
			var value = options.formatMatch(rawValue, i+1, options.data.length);
			if ( value === false )
				continue;
				
			var firstChar = value.charAt(0).toLowerCase();
			// if no lookup array for this character exists, look it up now
			if( !stMatchSets[firstChar] ) 
				stMatchSets[firstChar] = [];

			// if the match is a string
			var row = {
				value: value,
				data: rawValue,
				result: options.formatResult && options.formatResult(rawValue) || value
			};
			
			// push the current match into the set list
			stMatchSets[firstChar].push(row);

			// keep track of minChars zero items
			if ( nullData++ < options.max ) {
				stMatchSets[""].push(row);
			}
		};

		// add the data items to the cache
		$.each(stMatchSets, function(i, value) {
			// increase the cache size
			options.cacheLength++;
			// add to the cache
			add(i, value);
		});
	}
	
	// populate any existing data
	setTimeout(populate, 25);
	
	function flush(){
		data = {};
		length = 0;
	}
	
	return {
		flush: flush,
		add: add,
		populate: populate,
		load: function(q) {
			if (!options.cacheLength || !length)
				return null;
			/* 
			 * if dealing w/local data and matchContains than we must make sure
			 * to loop through all the data collections looking for matches
			 */
			if( !options.url && options.matchContains ){
				// track all matches
				var csub = [];
				// loop through all the data grids for matches
				for( var k in data ){
					// don't search through the stMatchSets[""] (minChars: 0) cache
					// this prevents duplicates
					if( k.length > 0 ){
						var c = data[k];
						$.each(c, function(i, x) {
							// if we've got a match, add it to the array
							if (matchSubset(x.value, q)) {
								csub.push(x);
							}
						});
					}
				}				
				return csub;
			} else 
			// if the exact item exists, use it
			if (data[q]){
				return data[q];
			} else
			if (options.matchSubset) {
				for (var i = q.length - 1; i >= options.minChars; i--) {
					var c = data[q.substr(0, i)];
					if (c) {
						var csub = [];
						$.each(c, function(i, x) {
							if (matchSubset(x.value, q)) {
								csub[csub.length] = x;
							}
						});
						return csub;
					}
				}
			}
			return null;
		}
	};
};

$.Autocompleter.Select = function (options, input, select, pintar, config) {
	var CLASSES = {
		ACTIVE: "ac_over"
	};
	
	var listItems,
		active = -1,
		data,
		term = "",
		needsInit = true,
		element,
		list;
	
	// Create results
	function init() {
		if (!needsInit)
			return;
		element = $("<div/>")
		.hide()
		.addClass(options.resultsClass)
		.css("position", "absolute")
		//.appendTo(document.body);

        if (typeof panelContAutoComplet != 'undefined') {
            element.appendTo($("#" + panelContAutoComplet));
        }
        else
        {
        	element.appendTo($(input).parent());
            //element.appendTo(document.body);
        }
	
		list = $("<ul/>").appendTo(element).mouseover( function(event) {
			if(target(event).nodeName && target(event).nodeName.toUpperCase() == 'LI') {
	            active = $("li", list).removeClass(CLASSES.ACTIVE).index(target(event));
			    $(target(event)).addClass(CLASSES.ACTIVE);
	        }
		}).click(function(event) {
			$(target(event)).addClass(CLASSES.ACTIVE);
			select();
			// TODO provide option to avoid setting focus again after selection? useful for cleanup-on-focus
			input.focus();
			return false;
		}).mousedown(function(event) {
            cancelEvent(event);
			config.mouseDownOnSelect = true;
		}).mouseup(function() {
			config.mouseDownOnSelect = false;
		});
		
		if( options.width > 0 )
			element.css("width", options.width);
			
		if (options.extraParams.maxwidth)
		    element.css("max-width", options.extraParams.maxwidth);
			
		needsInit = false;
	} 
	
	function target(event) {
		var element = event.target;
		while(element && element.tagName != "LI")
			element = element.parentNode;
		// more fun with IE, sometimes event.target is empty, just ignore it then
		if(!element)
			return [];
		return element;
	}

	function moveSelect(step) {
		listItems.slice(active, active + 1).removeClass(CLASSES.ACTIVE);
		movePosition(step);
        var activeItem = listItems.slice(active, active + 1).addClass(CLASSES.ACTIVE);
        if(options.scroll) {
            var offset = 0;
            listItems.slice(0, active).each(function() {
				offset += this.offsetHeight;
			});
            if((offset + activeItem[0].offsetHeight - list.scrollTop()) > list[0].clientHeight) {
                list.scrollTop(offset + activeItem[0].offsetHeight - list.innerHeight());
            } else if(offset < list.scrollTop()) {
                list.scrollTop(offset);
            }
        } 

//	    try
//	    {
//            pintar($(input), activeItem.html());
//        }
//        catch(ex)
//        {}
		//$(input).val($(input).val().replace(lastWord($(input).val()), QuitarContadores(activeItem.html())));
		//$(input).val(QuitarContadores(activeItem.html()));     
	};
	
	function movePosition(step) {
		active += step;
		if (active < 0) {
			active = listItems.size() - 1;
		} else if (active >= listItems.size()) {
			active = 0;
		}
	}
	
	function limitNumberOfItems(available) {
		return options.max && options.max < available
			? options.max
			: available;
	}
	
	function fillList() {
		list.empty();
		var max = limitNumberOfItems(data.length);
		for (var i=0; i < max; i++) {
			if (!data[i])
				continue;
			var formatted = options.formatItem(data[i].data, i+1, max, data[i].value, term);
			if ( formatted === false )
				continue;
			var li = $("<li/>").html( options.highlight(formatted, term) ).addClass(i%2 == 0 ? "ac_even" : "ac_odd").appendTo(list)[0];
			$.data(li, "ac_data", data[i]);
		}
		listItems = list.find("li");
		if ( options.selectFirst ) {
			listItems.slice(0, 1).addClass(CLASSES.ACTIVE);
			active = 0;
		}
		// apply bgiframe if available
		if ( $.fn.bgiframe )
			list.bgiframe();
	}
	
	return {
		display: function(d, q) {
			init();
			data = d;
			term = q;
			fillList();
		},
		next: function() {
			moveSelect(1);
		},
		prev: function() {
			moveSelect(-1);
		},
		pageUp: function() {
			if (active != 0 && active - 8 < 0) {
				moveSelect( -active );
			} else {
				moveSelect(-8);
			}
		},
		pageDown: function() {
			if (active != listItems.size() - 1 && active + 8 > listItems.size()) {
				moveSelect( listItems.size() - 1 - active );
			} else {
				moveSelect(8);
			}
		},
		hide: function() {
			$('body').removeClass('autocompletandoPrincipal');
			element && element.hide();
			listItems && listItems.removeClass(CLASSES.ACTIVE);
			active = -1;
		},
		visible : function() {
			return element && element.is(":visible");
		},
		current: function() {
			return this.visible() && (listItems.filter("." + CLASSES.ACTIVE)[0] || options.selectFirst && listItems[0]);
		},
		show: function() {
			if (input.id == "txtBusquedaPrincipal") $('body').addClass('autocompletandoPrincipal');
            if (typeof panelContAutoComplet != 'undefined') {
                element.css('display','block');
            }
            else
            {
			    var offset = $(input).offset();
			    element.css({
				    width: typeof options.width == "string" || options.width > 0 ? options.width : $(input).width(),
				    top: offset.top + input.offsetHeight,
				    left: offset.left
			    }).show();
            }
            if(options.scroll) {
                list.scrollTop(0);
                list.css({
					maxHeight: options.scrollHeight,
					overflow: 'auto'
				});
				
//                if($.browser.msie && typeof document.body.style.maxHeight === "undefined") {
//					var listHeight = 0;
//					listItems.each(function() {
//						listHeight += this.offsetHeight;
//					});
//					var scrollbarsVisible = listHeight > options.scrollHeight;
//                    list.css('height', scrollbarsVisible ? options.scrollHeight : listHeight );
//					if (!scrollbarsVisible) {
//						// IE doesn't recalculate width when scrollbar disappears
//						listItems.width( list.width() - parseInt(listItems.css("padding-left")) - parseInt(listItems.css("padding-right")) );
//					}
//                }
                
            }
		},
		selected: function() {
			var selected = listItems && listItems.filter("." + CLASSES.ACTIVE).removeClass(CLASSES.ACTIVE);
			return selected && selected.length && $.data(selected[0], "ac_data");
		},
		emptyList: function (){
			list && list.empty();
		},
		unbind: function() {
			element && element.remove();
		}
	};
};

$.fn.selection = function(start, end) {
	if (start !== undefined) {
		return this.each(function() {
			if( this.createTextRange ){
				var selRange = this.createTextRange();
				if (end === undefined || start == end) {
					selRange.move("character", start);
					selRange.select();
				} else {
					selRange.collapse(true);
					selRange.moveStart("character", start);
					selRange.moveEnd("character", end);
					selRange.select();
				}
			} else if( this.setSelectionRange ){
				this.setSelectionRange(start, end);
			} else if( this.selectionStart ){
				this.selectionStart = start;
				this.selectionEnd = end;
			}
		});
	}
	var field = this[0];
	if ( field.createTextRange && document.selection && document.selection.createRange ) {
		var range = document.selection.createRange(),
			orig = field.value,
			teststring = "<->",
			textLength = range.text.length;
		range.text = teststring;
		var caretAt = field.value.indexOf(teststring);
		field.value = orig;
		this.selection(caretAt, caretAt + textLength);
		return {
			start: caretAt,
			end: caretAt + textLength
		}
	} else if( field.selectionStart !== undefined ){
		return {
			start: field.selectionStart,
			end: field.selectionEnd
		}
	}
};
})(jQuery);


function PintarTags(textBox)
{
    if(textBox.val().trim() != "")
    {
        var tags = textBox.val().replace(';', ',').split(',');
        
        var contenedor = textBox.parents('.autocompletar').find('.contenedor');
        var textBoxHack = textBox.parents('.autocompletar').find('input').last();
        
        if(textBoxHack.length > 0)
        {
            for(var i=0; i<tags.length; i++)
            {
                var tagNombre = tags[i].trim();
                var tagNombreEncode = Encoder.htmlEncode(tagNombre);
                
                var estaYaAgregada = textBoxHack.val().trim().indexOf(',' + tagNombre + ',') != -1;
                estaYaAgregada = estaYaAgregada || textBoxHack.val().trim().substring(0, tagNombre.length + 1) == tagNombre + ',';
                
                if(tagNombre != '' && (!estaYaAgregada || textBox.parents('.tag').length > 0))
                {
                    var html = "<div class=\"tag\" title=\"" + tagNombreEncode + "\"><div>" + tagNombre + "<a class=\"remove\" ></a></div><input type=\"text\" value=\"" + tagNombreEncode + "\"></div>";
                    if(textBox.parents('.tag').length > 0)
                    {
                        textBox.parents('.tag').before(html);
                    }
                    else
                    {
	                    contenedor.append(html);  
                    }
                    
                    textBoxHack.val(textBoxHack.val() + tagNombre.toLowerCase() + ',')
                }
            }
            
            textBox.val('');
            
            if(textBox.parents('.tag').length == 0)
            {
                PosicionarTextBox(textBoxHack.prev());
            }
            
            if(!textBoxHack.prev().hasClass("no-edit"))
            {
                textBox.parents('.autocompletar').find('.tag').each(function(){
                    $(this).bind('click', function(evento){
                        cancelEvent(evento);
                         
                        var divTag = $(this).children('div');
                        var textBox = divTag.parent().find('input');
                        if(textBox.css('display') == 'none')
                        {
                            textBox.width(textBox.parent().width());
                            divTag.css('display', 'none');
                            textBox.css('display', 'block');
                            textBox.focus();
                            posicionarCursor(textBox,textBox.val().length);
                            textBox.blur(function(){ActualizarTag(textBox, divTag, textBoxHack)});
                            textBox.keydown(function(evento){
                                $(this).attr('size',$(this).val().length + 5);
                                if(evento.which || evento.keyCode){
                                    if ((evento.which == 13) || (evento.keyCode == 13)) {
                                        ActualizarTag(textBox, divTag, textBoxHack);
                                        return false;
                                    }
                                }
                            });
                            textBox.keyup(function(evento){
                                if(evento.which || evento.keyCode){
                                    if ((evento.which == 188) || (evento.keyCode == 188)) {
                                        ActualizarTag(textBox, divTag, textBoxHack);
                                    }
                                }
                            });
                        }
                    });
                });  
            }
            
             textBox.parents('.autocompletar').find('.tag .remove').each(function(){
                if($(this).data("events") == null)
                {
                    $(this).bind('click', function(evento){
                        cancelEvent(evento);
                        EliminarTag($(this).parents('.tag'), evento)
                    });
                }
            });
        }
    }
    tagYaPintado = true;
}

function PosicionarTextBox(textBox)
{
    textBox.width(150);
    textBox.css('top', '0px');
    textBox.css('left', '0px');

    if (textBox.parent().find('.tag').length == 0 || textBox.position().top > textBox.parent().find('.tag').last().position().top)
    {
        textBox.css('width', '100%');
    }
    else
    {
        var tbLeft = textBox.parent().find('.tag').last().position().left + textBox.parent().find('.tag').last().width() + 5;
        textBox.width(textBox.parent().width() - (tbLeft - textBox.parent().position().left));
    }
}

function LimpiarTags(textBox)
{
    $('#' + txtTagsID + '_Hack').val('');
    $('#' + txtTagsID).parent().find('.tag').remove();
}

function ActualizarTag(textBox, divTag, textBoxHack)
{
    var ultimoElemento = textBoxHack.parent();
    if(ultimoElemento.next().hasClass('propuestos'))
    {
        ultimoElemento = ultimoElemento.next();
    }
    descartarTag(textBox.parents('.tag'), ultimoElemento);
    
    textBox.css('display', '');
    PintarTags(textBox);
    
    var valorAnterior = textBox.parents('.tag').attr('title').toLowerCase();
    var hack = textBoxHack.val().trim();
    
    if(hack.indexOf(',' + valorAnterior + ',') != -1)
    {
        hack = hack.replace(',' + valorAnterior + ',', ',');
    }
    else if(hack.substring(0, valorAnterior.length + 1) == valorAnterior + ',')
    {
        hack = hack.replace(valorAnterior + ',', '');
    }
                
    textBoxHack.val(hack.trim());
    textBox.parent().remove();
    PosicionarTextBox(textBoxHack.prev());
}

function EliminarTag(elemento, evento)
{
    var divAutocompletar = elemento.parents('.autocompletar');
    if(divAutocompletar.find('input').length > 0)
    {
        var valorAnterior = elemento.attr('title');
        var textBoxHack = divAutocompletar.find('input').last();
        textBoxHack.val(textBoxHack.val().replace(valorAnterior.toLowerCase() + ',', ''));
        var ultimoElemento = divAutocompletar;
        if(divAutocompletar.next().hasClass('propuestos'))
        {
            var listaPropuestos = divAutocompletar.next().find('.tag');

            for(var i=0;i<listaPropuestos.length;i++)
            {
                if($(listaPropuestos[i]).attr('title') == valorAnterior)
                {
                    $(listaPropuestos[i]).css('display', '');
                }
            }
            ultimoElemento = divAutocompletar.next();
        }
        
        descartarTag(elemento, ultimoElemento);
        
        elemento.remove();
        PosicionarTextBox(textBoxHack.prev());
    }
}

function descartarTag(elemento, ultimoElemento)
{
    if(!ultimoElemento.next().hasClass('descartados'))
    {
        ultimoElemento.after("<div class='descartados' style='display:none;'><input id='txtHackDescartados' type='text'/></div>");
        ultimoElemento.next().find('#txtHackDescartados').val(elemento.attr('title').toLowerCase() + ',');
    }
    else
    {
        var descartados = ultimoElemento.next().find('#txtHackDescartados').val();
        
        var estaYaAgregada = descartados.indexOf(',' + elemento.attr('title') + ',') != -1;
        estaYaAgregada = estaYaAgregada || descartados.substring(0, elemento.attr('title').length + 1) == elemento.attr('title') + ',';
        
        if(!estaYaAgregada)
        {
            descartados += elemento.attr('title').toLowerCase() + ','
            ultimoElemento.next().find('#txtHackDescartados').val(descartados);
        }
    }
}

function posicionarCursor(textbox, pos) {
    if (textbox.get(0).setSelectionRange) {
        textbox.get(0).setSelectionRange(pos, pos);
    } else if (textbox.get(0).createTextRange) {
        var range = textbox.get(0).createTextRange();
        range.collapse(true);
        range.moveEnd('character', pos);
        range.moveStart('character', pos);
        range.select();
    }
}

$(document).ready(function() {
    pintarTagsInicio();
});

function pintarTagsInicio()
{
    $('.autocompletar').each(function(){
        $(this).bind('click', function(evento){
            cancelEvent(evento);
            $(this).find('input.txtAutocomplete').focus();
        });
    });
    
    $('.autocompletar input.txtAutocomplete').each(function(){
        PosicionarTextBox($(this));
        $(this).bind('keydown', function(evento){
            if ((evento.which == 8) || (evento.keyCode == 8)) {
                if($(this).val() == "")
                {
                    if($(this).parent().find('.tag').last().hasClass("selected"))
                    {
                        EliminarTag($(this).parent().find('.tag').last(), evento);
                    }
                    else
                    {
                        $(this).parent().find('.tag').last().addClass("selected");
                    }
                    return false;
                }
            }
            else if ((evento.which == 9) || (evento.keyCode == 9) || (evento.which == 13) || (evento.keyCode == 13)) {
                //Tabulador o Intro
                return false;
            }
            else
            {
                if($(this).parent().find('.tag').last().hasClass("selected"))
                {
                    $(this).parent().find('.tag').last().removeClass("selected");
                }
            }
        });
        $(this).bind('blur', function(evento){            
            PintarTags($(this));
        });
        $(this).bind('click', function(evento){
            cancelEvent(evento);
        });
        PintarTags($(this));
    });
}


function cancelEvent(e) {
    if (!e) e = window.event;
    if (e.preventDefault) {
        e.preventDefault();
    } else {
        e.returnValue = false;
    }
        
    if (!e) e = window.event;
    if (e.stopPropagation) {
        e.stopPropagation();
    } else {
        e.cancelBubble = true;
    }
}

function ObtenerUrlMultiple(pOptions){
    if (pOptions.servicio != null && pOptions.servicio.service.indexOf(',') != -1)
    {
        pOptions.urlParteAsmx = pOptions.servicio.service.substring(pOptions.servicio.service.lastIndexOf(',') + 1);
        pOptions.urlmultiple = pOptions.servicio.service.substring(0, pOptions.servicio.service.lastIndexOf(',')).split(',');
        pOptions.urlActual = aleatorio(0, pOptions.urlmultiple.length - 1);
    }
    else if (pOptions.url != null && pOptions.url.indexOf(',') != -1) {
        pOptions.urlParteAsmx = pOptions.url.substring(pOptions.url.lastIndexOf(',') + 1);
        pOptions.urlmultiple = pOptions.url.substring(0, pOptions.url.lastIndexOf(',')).split(',');
        pOptions.urlActual = aleatorio(0, pOptions.urlmultiple.length - 1);
    }
}

function aleatorio(inferior,superior){ 
    numPosibilidades = superior - inferior;
    aleat = Math.random() * numPosibilidades;
    aleat = Math.round(aleat);
    return parseInt(inferior) + aleat;
}