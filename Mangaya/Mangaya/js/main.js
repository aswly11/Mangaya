$(document).ready(function(){
    
        $("#entry").fadeOut(1000,function(){

        $("body").css("overflow","auto")
        
    })
    
})

$(document).ready(function(){
    
    /*$(".logo").animate({left:'200px'},1000);*/
    
})




$(window).scroll(function(){
    
    var wScroll=$(window).scrollTop();
    
    if(wScroll>200){
        
        $(".na").css("opacity",".7")
    
    }
    else 
        
        {
            
            $(".na").css("opacity","1")
        }
    
    
    
})






