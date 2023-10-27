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

    $('#createModal').on('show.bs.modal', function (e) {
        // do something...    
        purchaseStatusChanged('PurchaseStatus', 'new');
        readingStatusChanged('Status', 'new')
    })

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

function fillFilters() {
    var sort = $('select[id=sortType]').val()
    var dir = $('button[id=sortDirection').data('sort')
    var search = $('input[id=search-field]').val()
    var digitality = $('input[type=radio][name=digitality]:checked').attr('data')
    var reading = $('input[type=radio][name=reading]:checked').attr('data');
    var filter = $('input[type=radio][name=filter]:checked').attr('data')


    return {
        "filter": filter,
        "sort": sort,
        "direction": dir,
        "search": search,
        "digitality": digitality,
        "reading": reading
    };
}

function selectRadioButtonByValue(name, value) {
    var radioButtons = document.querySelectorAll('input[type=radio][name='+name+']');

    radioButtons.forEach(function (radio) {
        if (radio.value == value) {
            radio.checked = true; // Select the radio button if the value matches
        }
    });
}

function fillFields(filters) {
    $('select[id=sortType]').val(filters.sort)
    $('button[id=sortDirection').data('sort', filters.dir)
    $('input[id=search-field]').val(filters.search)
    selectRadioButtonByValue('digitality', filters.digitality)
    selectRadioButtonByValue('reading', filters.reading)
    selectRadioButtonByValue('filter', filters.filter)
}

function filter(e) {

    var filters;

    if (e != undefined && e.target.selectedOptions != undefined) {
        filters = JSON.parse(e.target.selectedOptions[0].dataset.json);

        if (filters == undefined) {
            filters = fillFilters();
        }
        else {
            fillFields(filters);
        }
    }
    else {
        filters = fillFilters();
    }


    ////var existing = _.isEqual(selFilter, filters)
    //if (existing) {
    //    //selectfilter in Dropdown

    //}
    //else {
    //    $("#filter-presets").val(0);
    //}

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
    $.ajax({
        url: "?handler=Volume",
        type: 'GET',
        cache: false,
        data: { id: id },

    }).done(function (result) {
        $('#detail-modal-content').html(result);
        purchaseStatusChanged('PurchaseStatus')
        readingStatusChanged('Status')
    });
}



function refillFilterDropdown(e) {
    if (e.status == 200) {

        $("#filterName").val('');

        $ddl = $("#filter-presets");
        $ddl.find('option').not(':first').remove();


        for (var i = 0; i < e.responseJSON.length; i++) {
            var item = e.responseJSON[i];
            $ddl.append($("<option/>").val(item.id).html(item.name).attr('class', 'bg-secondary').attr('data-json', item.json));
            if (item.selected) {
                $ddl.val(item.id);
            }
        }

    }
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
                $('#detailModal').modal('hide');
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

        resetPreview("new-volume-cover");


        if (document.getElementById("add-more").checked) {
            $('#NewVolume_Number').val(parseInt($('#NewVolume_Number').val()) + 1).trigger('change');
            document.getElementById('NewVolume_CoverFile').value = "";
            $('input[name="NewVolume.Rating"]').prop('checked', false);
        }
        else {
            document.getElementById("create-form").reset();
            resetValidation();
            resetPreview("new-volume-cover");
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

function fieldVisibility(fieldId, visibility, prefix) {
    if (prefix != undefined) {
        fieldId = '#' + prefix + '-' + fieldId;
    }
    else {
        fieldId = '#' + fieldId;
    }

    if (visibility) {
        $(fieldId).removeClass("collapse");
    }
    else {
        $(fieldId).addClass("collapse");
    }
}

function attachPrefix(input, prefix) {
    if (prefix != undefined) {
        return '#' + prefix + '-' + input;
    } else {
        return '#' + input;
    }
}

function purchaseStatusChanged(purchseStatus, prefix) {

    if ($.type(purchseStatus) === "string") {
        purchseStatus = attachPrefix(purchseStatus, prefix);
    }

    var value = $(purchseStatus).find(":selected").val();
    switch (value) {
        case "Announced":
            fieldVisibility("release-date", true, prefix);
            fieldVisibility("preorder-date", false, prefix);
            fieldVisibility("purchase-date", false, prefix);
            fieldVisibility("reading-status", false, prefix);
            fieldVisibility('rating-select', false, prefix);
            break;
        case "Preordered":
            fieldVisibility("release-date", true, prefix);
            fieldVisibility("preorder-date", true, prefix);
            fieldVisibility("purchase-date", false, prefix);
            fieldVisibility("reading-status", false, prefix);
            fieldVisibility('rating-select', false, prefix);
            break;
        case "Wishlist":
            fieldVisibility("release-date", false, prefix);
            fieldVisibility("preorder-date", false, prefix);
            fieldVisibility("purchase-date", false, prefix);
            fieldVisibility("reading-status", false, prefix);
            fieldVisibility('rating-select', false, prefix);
            break;
        default:
            fieldVisibility("release-date", false, prefix);
            fieldVisibility("preorder-date", false, prefix);
            fieldVisibility("purchase-date", true, prefix);
            fieldVisibility("reading-status", true, prefix);
            break;
    }
}


function readingStatusChanged(list, prefix) {

    if ($.type(list) === "string") {
        list = attachPrefix(list, prefix);
    }

    var value = $(list).find(":selected").val();

    switch (value) {
        case "Completed":
            fieldVisibility('rating-select', true, prefix);
            break;
        case "Dropped":
            fieldVisibility('rating-select', true, prefix);
            break;
        default:
            fieldVisibility('rating-select', false, prefix);
            break;

    }
}


function resetCreateForm() {
    setTimeout(function () {
        resetValidation();
        resetPreview('new-volume-cover');
        purchaseStatusChanged('PurchaseStatus', 'new');
        readingStatusChanged('Status', 'new');

    }, 10);

}

function showNewPreview(event) {
    if (event.target.files.length > 0) {
        var src = URL.createObjectURL(event.target.files[0]);
        var preview = document.getElementById("new-volume-cover");
        preview.src = src;
    }
}

function showPreview(event) {
    if (event.target.files.length > 0) {
        var src = URL.createObjectURL(event.target.files[0]);
        var preview = document.getElementById("volume-cover");
        preview.src = src;
    }
}

function resetPreview(previewId) {
    document.getElementById(previewId).src = "images\\static\\no-cover.png";

}