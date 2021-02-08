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

var linkTabs = {
  init: function(){
    this.config();
    this.comportamiento();
    return;
  },
  config: function(){
    this.body = body;
    this.header = this.body.find('.navbar');
    this.tabs = this.body.find('.nav-tabs');
    return;
  },
  comportamiento: function(){
    
    var hash = window.location.hash;
    var hashSplit = hash.substr(0, 1);
    var seccion = hash.substr(1);
    
    if(hashSplit == "#"){
      var tab = this.tabs.find('#' + seccion + '-tab');
      tab.trigger('click');
    }

    window.addEventListener('hashchange',()=>{
      var newhash = window.location.hash;
      var newhashSplit = newhash.substr(0, 1);
      var seccion = newhash.substr(1);
      if(newhashSplit == "#"){
        var tab = this.tabs.find('#' + seccion + '-tab');
        tab.trigger('click');
      }
    });

    return;
  }
};

var body;

$(document).ready(function () {

    body = $('body');

    bodyScrolling.init();
    limpiarEstilosInline.init();

    if(body.hasClass('tipo-documento')){
      linkTabs.init();
    }
});