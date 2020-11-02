var xsize1 = document.body.width;
var xsize2 = window.innerWidth;

drawcanvas.width = drawarea.offsetWidth;
drawcanvas.height = drawarea.offsetHeight;
var canvas = document.createElement("CANVAS");
var drawcanvas = document.getElementById("drawcanvas");
m_ctxDraw = drawcanvas.getContext('2d');
