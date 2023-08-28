$(function () {

    $('#Volume_Series').autocomplete({
        minLength: 0,
        source: function (request, response) {
            $.getJSON("?handler=SearchSeries", {
                term: request.term
            }, response);
        },
    });



   

});