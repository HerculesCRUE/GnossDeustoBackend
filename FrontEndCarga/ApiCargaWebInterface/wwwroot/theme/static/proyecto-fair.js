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

var body;

$(document).ready(function () {

    body = $('body');

    bodyScrolling.init();
    limpiarEstilosInline.init();
});