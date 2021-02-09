var limpiarEstilosInline = {
    init: function() {
        this.config();
        this.borrarEstilos();
    },
    config: function() {
        this.body = $('body');
        this.contenidoPrincipal = this.body.find('#contenido-principal');
        this.elementos = this.contenidoPrincipal.find('*');
    },
    borrarEstilos: function() {
        this.elementos.removeAttr('style');
    }
}

$(document).ready(function () {
    limpiarEstilosInline.init();
});