$.validator.addMethod("reqif",
    function (value, element, parameters) {
        var onging = $("#NewVolume_PurchaseStatus").is(":checked");
        return onging == false;
    });

$.validator.unobtrusive.adapters.add("reqif", [], function (options) {
    options.rules.reqif = {};
    options.messages["reqif"] = options.message;
});

$(function () {

    $('.filter').each(function () {
        //$(this).prop('checked', true)
        $(this).on('click', filter)
    });

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

    $(".multple-datalist").focusin(function () { $(this).attr("type", "email"); });
    $(".multple-datalist").focusout(function () { $(this).attr("type", "textbox"); });

    purchaseStatusChanged()
    newReadingStatusChanged()
});

function resetFilters(e) {

    $('#Purchase_All').prop("checked", "checked")
    $('#Digitality_All').prop("checked", "checked")
    $('#Reading_All').prop("checked", "checked")
    $('#sortType').val("ByPurchaseDate")
    $('#search-field').val("")
    $('#sortDirection').attr('data-sort', "up");
    $('#sortDirection').data('sort', 'up').find('.bi').attr('class', 'bi bi-sort-up');

    filter(e);
}

function filter(e) {

    var sort = $('select[id=sortType]').val()
    var dir = $('button[id=sortDirection').data('sort')
    var search = $('input[id=search-field]').val()
    var digitality = $('input[type=radio][name=digitality]:checked').attr('data')
    var reading = $('input[type=radio][name=reading]:checked').attr('data');
    //if (reading != "All") {
    //    console.log(reading)
    //    var radio = $('#Purchase_Available')
    //    radio.prop("checked", "checked");

    //}
    var filter = $('input[type=radio][name=filter]:checked').attr('data')


    var filters = {
        "filter": filter,
        "sort": sort,
        "direction": dir,
        "search": search,
        "digitality": digitality,
        "reading": reading
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
        existhigPurchaseStatusChanged()
        existhigReadingStatusChanged()
    });
}





function changeStatusSuccess(e) {
    $('#PurchaseStatus').removeClass("text-danger");

    if (e.status == 200) {
        // Get all elements with the class "update-on-status-change"
        var elements = document.querySelectorAll('.update-on-status-change');

        // Loop through each element
        elements.forEach(function (element) {
            // Add the "text-success" class with a delay
            element.classList.add('text-success');

            // Remove the "text-success" class after a delay
            setTimeout(function () {
                element.classList.remove('text-success');
            }, 1000);
        });

    }
    else {
        $('#PurchaseStatus').addClass("text-danger");
    }
}

function updateTitle(title) {
    $('#NewVolume_Title').val(title + " " + $('#NewVolume_Number').val());
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

    console.log("createComplete");

    if (xnr.status == 200) {
        $('#createAlert').hide();

        resetPreview();
      

        if (document.getElementById("add-more").checked) {
            $('#NewVolume_Number').val(parseInt($('#NewVolume_Number').val()) + 1).trigger('change');
            document.getElementById('NewVolume_CoverFile').value = "";
        }
        else {           
            document.getElementById("create-form").reset();
            $('#createModal').modal('hide')
        }

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

function purchaseStatusChanged() {
    var value = $("#NewVolume_PurchaseStatus").find(":selected").val();

    if (value == "Preordered" || value == "Announced") {
        $('#release-date').removeClass("collapse");
    }
    else {
        $('#release-date').addClass("collapse");
    }

    if (value == "Announced" || value == "Wishlist") {

        $('#purchase-date').addClass("collapse");
    }
    else {
        $('#purchase-date').removeClass("collapse");
    }
}

function existhigPurchaseStatusChanged() {
    var value = $("#PurchaseStatus").find(":selected").val();

    if (value == "Preordered" || value == "Announced") {
        $('#new-release-date').removeClass("collapse");
        $('#reading-status').addClass("collapse");
    }
    else {
        $('#new-release-date').addClass("collapse");
        $('#reading-status').removeClass("collapse");
    }

    //new-purchase-date
    if (value == "Announced" || value == "Wishlist") {

        $('#new-purchase-date').addClass("collapse");
    }
    else {
        $('#new-purchase-date').removeClass("collapse");
    }
}

function existhigReadingStatusChanged() {

    var value = $("#Status").find(":selected").val();

    if (value == "Completed" || value == "Dropped") {
        $('#rating-select').removeClass("collapse");
    }
    else {
        $('#rating-select').addClass("collapse");
    }
}
function newReadingStatusChanged() {

    var value = $("#NewVolume_Status").find(":selected").val();

    if (value == "Completed" || value == "Dropped") {
        $('#new-rating-select').removeClass("collapse");
    }
    else {
        $('#new-rating-select').addClass("collapse");
    }
}

function showPreview(event) {
    if (event.target.files.length > 0) {
        var src = URL.createObjectURL(event.target.files[0]);
        var preview = document.getElementById("volume-cover");
        preview.src = src;
    }
}

function resetPreview() {
    document.getElementById("volume-cover").src = "images\\static\\no-cover.png";
}