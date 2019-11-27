// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function highlight2(text) {
    var keyword = document.getElementById("searchBox").value;
    console.log(keyword);
    var inputText = document.getElementById("transcript");
    var innerHTML = inputText.innerHTML;
    var index = innerHTML.indexOf(keyword);
    if (index >= 0) {
        innerHTML = innerHTML.substring(0,index) + "<span class='highlight'>" + innerHTML.substring(index,index+text.length) + "</span>" + innerHTML.substring(index + text.length);
        inputText.innerHTML = innerHTML;
    }
}

var opar = document.getElementById('transcript').innerHTML;

function highlight() {
    var paragraph = document.getElementById('transcript');
    var search = document.getElementById('searchBox').value;
    search = search.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); 

    var re = new RegExp(search, 'g');
    var m;

    if (search.length > 0)
        paragraph.innerHTML = opar.replace(re, `<span class='highlight'>$&</span>`);
    else paragraph.innerHTML = opar;
}