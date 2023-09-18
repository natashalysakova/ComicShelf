$(function () {

    $('.ps-filter').each(function () {
        $(this).prop('checked', true)
        $(this).on('click', function (e) {

            var all = $('#FilterAll')[0];
            var value = e.target.checked

            if (all == e.target) {


                $('.ps-filter').each(function () {
                    $(this).prop("checked", value);
                });
            }
            else {
                if (!value)
                    all.checked = false;
                else {
                    if ($('.ps-filter:checked').length == $('.ps-filter').length - 1) {
                        all.checked = true;
                    }
                }
            }

            var PurchaseFilters = {

                "FilterAvailable": $('#FilterAvailable')[0].checked,
                "FilterPreorder": $('#FilterPreorder')[0].checked,
                "FilterWishlist": $('#FilterWishlist')[0].checked,
                "FilterAnnounced": $('#FilterAnnounced')[0].checked,
                "FilterGone": $('#FilterGone')[0].checked
            };

            $.ajax({
                url: "?handler=Filtered",
                type: 'GET',
                //dataType: 'json',
                //contentType: "application/json; charset=utf-8",
                cache: false,
                data: PurchaseFilters
            }).done(function (result) {
                $('#shelves').empty().html(result);
            }).fail(function () {
            //    alert("Sorry. Server unavailable. ");
            });

        });
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

