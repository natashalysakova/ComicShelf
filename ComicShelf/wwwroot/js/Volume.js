$(function () {

    $('#SelectedSeries').autocomplete({
        minLength: 0,
        source: function (request, response) {
            $.getJSON("?handler=Search", {
                term: request.term
            }, response);
        },
    });
});