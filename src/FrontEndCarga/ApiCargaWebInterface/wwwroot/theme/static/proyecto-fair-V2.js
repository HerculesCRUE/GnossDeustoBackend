var estructurarContenidoPrincipal = {
    init: function() {
        this.config();
        this.borrarEstilos();
        this.crearEstructura();
        this.comportamientoDesplegable();
    },
    config: function() {
        this.body = $('body');
        this.contenidoPrincipal = this.body.find('#contenido-principal');
        this.elementos = this.contenidoPrincipal.find('*');
        this.hijos = this.contenidoPrincipal.find('> *');
        this.contenidoWrapper = $('<div id="contenido-wrapper"></div>');
    },
    borrarEstilos: function() {
        this.elementos.removeAttr('style');
    },
    crearEstructura: function() {
        this.quitarEnlaces();
        this.crearCabecera();
        this.crearLista();
    },
    crearCabecera: function() {
        var h1 = this.contenidoPrincipal.find('h1');
        var h2 = this.contenidoPrincipal.find('h2');
        this.contenidoWrapper.append(h1);
        this.contenidoWrapper.append(h2);
    },
    crearLista: function() {
        var firstitem = true;
        var that = this;
        var itemsNumber = 1;
        var itemWrapper = $('<div></div>').addClass('item item-' + itemsNumber);
        that.hijos.each(function (index, element) {
            element = $(element);
            if(index < 4) return;

            if(element.is('h3')){
                if(firstitem){
                    firstitem = false;
                    itemWrapper.append(element)
                }else{
                    that.contenidoWrapper.append(itemWrapper);
                    itemsNumber++;
                    itemWrapper = $('<div></div>').addClass('item item-' + itemsNumber);
                    itemWrapper.append(element)
                }
            }else{
                if(element.html() != "" && !element.is('p')){
                    itemWrapper.append(element);
                }
            }
        });
        that.contenidoWrapper.append(itemWrapper);
        that.contenidoPrincipal.empty();
        that.contenidoPrincipal.append(that.contenidoWrapper);
        that.crearCabecera();
    },
    crearCabecera: function(){
        var items = this.contenidoPrincipal.find('.item');
        items.each(function (index, element) {
            var cabecera = $('<div></div>').addClass('cabecera-item');
            var item = $(element);
            var h3 = item.find('h3');
            var divImg = item.find('> div:first-of-type');
            divImg.addClass('estrellas');
            cabecera.append(h3);
            cabecera.append(divImg);
            item.prepend(cabecera);
        });
    },
    quitarEnlaces: function() {
        this.hijos.remove('p:last-child');
        this.hijos = this.contenidoPrincipal.find('> *');
    },
    comportamientoDesplegable: function() {
        var cabeceraItem = this.contenidoPrincipal.find('.item .cabecera-item');
        cabeceraItem.click(function (e) { 
            e.preventDefault();
            $(this).parent().toggleClass('desplegado');
        });
    }
}

$(document).ready(function () {
    estructurarContenidoPrincipal.init();
});