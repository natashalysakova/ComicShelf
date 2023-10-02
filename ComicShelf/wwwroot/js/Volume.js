$(function () {

    $('.ps-filter').each(function () {
        //$(this).prop('checked', true)
        $(this).on('click', filter)
    });

    $('#button-search').on('click', filter)
    $('#sortType').on('change', filter)
    $('#search-field').on('input', filter)

    $('#sortDirection').on('click', function (event) {
        event.preventDefault();

        var $this = $(this),
            sortDir = 'down';

        if ($this.data('sort') !== 'up') {
            sortDir = 'up';
        }

        $this.data('sort', sortDir).find('.bi').attr('class', 'bi bi-sort-' + sortDir);

        filter(event);

    });

    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }


    $('#Volume_Series').autocomplete({
        minLength: 0,
        source: function (request, response) {
            $.getJSON("?handler=SearchSeries", {
                term: request.term
            }, response);
        },
        //focus: function () {
        //    // prevent value inserted on focus
        //    return false;
        //},
        //select: function (event, ui) {
        //    var terms = split(this.value);
        //    // remove the current input
        //    terms.pop();
        //    // add the selected item
        //    terms.push(ui.item.label);
        //    // add placeholder to get the comma-and-space at the end
        //    terms.push("");
        //    this.value = terms.join(", ");
        //    return false;
        //}
    })

    //.on("keydown", function (event) {
    //    if (event.keyCode === $.ui.keyCode.TAB &&
    //        $(this).autocomplete("instance").menu.active) {
    //        event.preventDefault();
    //    }
    //})


    $(".multple-datalist").focusin(function () { $(this).attr("type", "email"); });
    $(".multple-datalist").focusout(function () { $(this).attr("type", "textbox"); });


    //NewVolume_Title

});

function filter (e) {

    var all = $('#FilterAll')[0];
    var value = e.target.checked

    var filter = $('input[type=radio][name=filter]:checked').attr('id')
    var sort = $('select[id=sortType]').val()
    var dir = $('button[id=sortDirection').data('sort')
    var search = $('input[id=search-field]').val()

    var filters = {
        "filter": filter,
        "sort": sort,
        "direction": dir,
        "search": search
    };

    $.ajax({
        url: "?handler=Filtered",
        type: 'GET',
        cache: false,
        data: filters
    }).done(function (result) {
        $('#shelves').html(result);
    });
};

function bookClick(id) {
    console.log("clicked");
    $.ajax({
        url: "?handler=Volume",
        type: 'GET',
        cache: false,
        data: { id: id },
        
    }).done(function (result) {
        $('#detail-modal-content').html(result);
    });
}

function changeStatusSuccess(e) {
    console.log(e);

    if (e.status == 200) {
        $('#PurchaseStatus').addClass("text-success");
    }
    else {
        $('#PurchaseStatus').addClass("text-danger");
    }
}

function updateTitle() {
    $('#NewVolume_Title').val("Том " + $('#NewVolume_Number').val());
}

function switchCover() {
    if ($('#fileRadio').prop("checked") == true) {
        $('#Cover').show();
        $('#NewVolume_Cover').hide();
    }
    else {
        $('#Cover').hide();
        $('#NewVolume_Cover').show()

    }
}

function createComplete(xnr) {

    if (xnr.status == 200) {
        $('#createAlert').hide();
        $('#createModal').modal('hide')
        var s = $('#shelves')
        s.html(xnr.responseText);
    }
    else {
        var al = $('#createAlert');
        al.text(xnr.responseText);

        al.show();
    }
}

function resetValidation() {
    var $form = $('form');

    //reset jQuery Validate's internals
    $form.validate().resetForm();

    //reset unobtrusive validation summary, if it exists
    $form.find("[data-valmsg-summary=true]")
        .removeClass("validation-summary-errors")
        .addClass("validation-summary-valid")
        .find("ul").empty();

    //reset unobtrusive field level, if it exists
    $form.find("[data-valmsg-replace]")
        .removeClass("field-validation-error")
        .addClass("field-validation-valid")
        .empty();

    $form.find("[data-valmsg-replace]").empty();

    return $form;
};

