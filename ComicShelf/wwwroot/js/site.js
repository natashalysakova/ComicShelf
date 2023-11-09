// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

    
    window.onscroll = function () {
        scrollFunction();
    };
    mybutton.addEventListener("click", backToTop);
});

function ongoingChanged() {
    if ($('#Series_Ongoing').is(":checked")) {
        $('#totalIssues').addClass("invisible");
    }
    else {
        $('#totalIssues').removeClass("invisible");
    }
}


let mybutton = document.getElementById("btn-back-to-top");


// When the user scrolls down 20px from the top of the document, show the button


function scrollFunction() {
    if (
        document.body.scrollTop > 20 ||
        document.documentElement.scrollTop > 20
    ) {
        mybutton.style.display = "block";
    } else {
        mybutton.style.display = "none";
    }
}
// When the user clicks on the button, scroll to the top of the document


function backToTop() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}