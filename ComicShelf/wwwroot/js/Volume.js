﻿$.validator.addMethod("reqif",
    function (value, element, parameters) {
        var onging = $("#NewVolume_PurchaseStatus").is(":checked");
        return onging == false;
    });

$.validator.unobtrusive.adapters.add("reqif", [], function (options) {
    options.rules.reqif = {};
    options.messages["reqif"] = options.message;
});

var createPopover

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
        }
    })

    $(".multple-datalist").focusin(function () { $(this).attr("type", "email"); });
    $(".multple-datalist").focusout(function () { $(this).attr("type", "textbox"); });

    $('#createModal').on('show.bs.modal', function (e) {
        // do something...    
        purchaseStatusChanged('PurchaseStatus', 'new');
        readingStatusChanged('Status', 'new')
    })

    var options = {
        html: true,
        //html element
        //content: $("#popover-content")
        content: $('[data-name="save-filter-form"]')
        //Doing below won't work. Shows title only
        //content: $("#popover-content").html()

    }
    var exampleEl = document.getElementById('save-popover')
    createPopover = new bootstrap.Popover(exampleEl, options)


    //read current filters from cookies and fill calues on the page
    const filters = getJSONFromCookie('filters');
    fillFields(filters);
    findMatch(filters);
});

function resetFilters(e) {

    $('#Purchase_All').prop("checked", "checked")
    $('#Digitality_All').prop("checked", "checked")
    $('#Reading_All').prop("checked", "checked")
    $('#VolumeTypes_All').prop("checked", "checked")
    $('#sortType').val(0)
    $('#search-field').val("")
    $('#sortDirection').attr('data-sort', "up");
    $('#sortDirection').data('sort', 'up').find('.bi').attr('class', 'bi bi-sort-up');
    $('#filter-presets').val(0)

    filter(e);
}

function fillFilters() {
    var sort = parseInt($('select[id=sortType]').val())
    var dir = $('button[id=sortDirection').data('sort')
    if (dir == 'up') {
        dir = 0;
    } else {
        dir = 1;
    }
    var search = replaceEmptyStringWithNull($('input[id=search-field]').val())
    var digitality = parseInt($('input[type=radio][name=digitality]:checked').val())
    var reading = parseInt($('input[type=radio][name=reading]:checked').val())
    var filter = parseInt($('input[type=radio][name=filter]:checked').val())
    var volumeType = parseInt($('input[type=radio][name=volumeType]:checked').val())

    return {
        "filter": filter,
        "sort": sort,
        "direction": dir,
        "search": search,
        "digitality": digitality,
        "reading": reading,
        "volumeType": volumeType
    };
}

function selectRadioButtonByValue(name, value) {
    var radioButtons = document.querySelectorAll('input[type=radio][name=' + name + ']');

    radioButtons.forEach(function (radio) {
        if (radio.value == value) {
            radio.checked = true; // Select the radio button if the value matches
        }
    });
}

function fillFields(filters) {
    $('select[id=sortType]').val(filters.sort)

    var direct = 'up';
    if (filters.direction == 1) {
        direct = 'down'
    }


    $('#sortDirection').attr('data-sort', direct);
    $('#sortDirection').data('sort', direct).find('.bi').attr('class', 'bi bi-sort-' + direct);

    $('input[id=search-field]').val(filters.search)
    selectRadioButtonByValue('digitality', filters.digitality)
    selectRadioButtonByValue('reading', filters.reading)
    selectRadioButtonByValue('filter', filters.filter)
    selectRadioButtonByValue('volumeType', filters.volumeType)
    
}

// Function to compare JSON objects
function areEqual(obj1, obj2) {
    // Convert objects to strings after sorting keys to ensure consistent ordering
    const stringified1 = JSON.stringify(obj1, Object.keys(obj1).sort());
    const stringified2 = JSON.stringify(obj2, Object.keys(obj2).sort());
    return stringified1 === stringified2;
}
function replaceEmptyStringWithNull(inputString) {
    // Trim whitespace and check if the string is empty
    if (inputString.trim() === '') {
        return null;
    }
    return inputString;
}

function findMatch(filters) {
    const selectElement = document.getElementById('filter-presets');
    const options = selectElement.getElementsByTagName('option');
    for (let i = 0; i < options.length; i++) {
        const option = options[i];
        const jsonAttribute = JSON.parse(option.getAttribute('data-json'));

        if (jsonAttribute == undefined) {
            continue;
        }
        // Compare the JSON data
        if (areEqual(jsonAttribute, filters)) {
            console.log('Match found:', option.textContent);
            selectElement.value = option.value;
            // Do something with the matched option
            return; // If you only want the first match
        }
    }
    selectElement.value = 0;
}

let previousSearch = "";
function filter(e) {


    var filters;

    if (e != undefined && e.target.id == 'search-field') {
        const currentValue = e.target.value;

        if (currentValue == previousSearch) {
            return;
        }

        if (currentValue.length == 1) {
            return;
        }
    }

    if (e != undefined && e.target.id == 'filter-presets') {
        if (e.target.value == 0) {
            resetFilters();
            return;
        }

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
    findMatch(filters);

    previousSearch = filters.search;

    $('#shelves').html('');
    $('#spinner').css('visibility', 'visible')


    $.ajax({
        url: "?handler=Filtered",
        type: 'GET',
        cache: false,
        data: filters
    }).done(function (result) {
        $('#shelves').html(result);
        $('#spinner').css('visibility', 'collapse')
    });
};

//function bookClick(id) {
//    $.ajax({
//        url: "/Manga/Index?handler=Volume",
//        type: 'GET',
//        cache: false,
//        data: { id: id },

//    }).done(function (result) {
//        $('#detail-modal-content').html(result);
//        purchaseStatusChanged('PurchaseStatus')
//        readingStatusChanged('Status')
//    });
//}


function detailsLoaded() {
    purchaseStatusChanged('PurchaseStatus')
    readingStatusChanged('Status')
}



function refillFilterDropdown(e) {
    if (e.status == 200) {

        if (createPopover != undefined) {
            createPopover.hide();
        }

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
    $("#update-spinner").hide();

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
        filter();
    }
    else {
        $('#PurchaseStatus').addClass("text-danger");
    }
}

function updateTitle(title, issue) {
    var selectedRadio = $("input[type='radio'][name='NewVolume.VolumeType']:checked").val();

    var newval;
    if (selectedRadio == "Issue")
        newval = issue + " " + $('#NewVolume_Number').val();
    else if (selectedRadio == "Volume")
        newval = title + " " + $('#NewVolume_Number').val();
    else
        newval = "Oneshot";

    $('#NewVolume_Title').val(newval);
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
    $('#import-spinner').hide();
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

            SetTodayDate("#ReleaseDate");
            break;
        case "Preordered":
            fieldVisibility("release-date", true, prefix);
            fieldVisibility("preorder-date", true, prefix);
            fieldVisibility("purchase-date", false, prefix);
            fieldVisibility("reading-status", false, prefix);
            fieldVisibility('rating-select', false, prefix);

            SetTodayDate("#PreorderDate");
            SetTodayDate("#ReleaseDate");
            break;
        case "Wishlist":
            fieldVisibility("release-date", false, prefix);
            fieldVisibility("preorder-date", false, prefix);
            fieldVisibility("purchase-date", false, prefix);
            fieldVisibility("reading-status", false, prefix);
            fieldVisibility('rating-select', false, prefix);

            SetTodayDate("#ReleaseDate");
            break;
        default:
            fieldVisibility("release-date", true, prefix);
            fieldVisibility("preorder-date", true, prefix);
            fieldVisibility("purchase-date", true, prefix);
            fieldVisibility("reading-status", true, prefix);
            fieldVisibility('rating-select', true, prefix);

            SetTodayDate("#PurchaseDate");
            SetTodayDate("#PreorderDate");
            SetTodayDate("#ReleaseDate");
            break;
    }
}

function GetTodayDate() {
    var today = new Date();
    var yyyy = today.getFullYear();
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var dd = String(today.getDate()).padStart(2, '0');
    var formattedDate = yyyy + '-' + mm + '-' + dd;
    return formattedDate;
}

function SetTodayDate(inputId) {
    var todaysDate = GetTodayDate();
    var releaseDate = $(inputId).val();
    if (releaseDate == '') {
        $(inputId).val(todaysDate);
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
        $('#NewVolume_CoverToDownload').val("");
        $('#NewVolume_Publisher').val("");
        $('#importUrl').val("");
        $('#imported-publisher').hide();
        $('#imported-isbn').hide();


    }, 10);

}

function showNewPreview(event) {
    if (event.target.files != null) {
        var src = URL.createObjectURL(event.target.files[0]);
        var preview = document.getElementById("new-volume-cover");
        preview.src = src;
    }
    else if (event.target.value != undefined) {
        var preview = document.getElementById("new-volume-cover");
        preview.src = event.target.value;
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

function getJSONFromCookie(cookieName) {
    const name = cookieName + '=';
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookieArray = decodedCookie.split(';');

    for (let i = 0; i < cookieArray.length; i++) {
        let cookie = cookieArray[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(name) === 0) {
            const jsonStr = cookie.substring(name.length, cookie.length);
            return JSON.parse(jsonStr);
        }
    }
    return null; // Return null if the cookie isn't found or doesn't contain valid JSON
}

function DeleteIssue(id) {

    $.ajax({
        url: "/Manga?handler=DeleteChapter",
        type: 'GET',
        cache: false,
        data: {
            "id": id
        },
        success: function (res) {
            var element = $('#issue_' + id);
            var parent = element.parent();
            var buttonAbove = parent.prev("button");

            element.remove();

            // Check if the parent is empty after removal
            if ($.trim(parent.html()) === "") {
                // If the parent is empty, remove it as well
                parent.remove();
                buttonAbove.remove();
            }
        },
        error: function (res) {
            alert('error!' + res);
        }
    });
}

function addChaptersSuccess(e) {

    if ($('#collapseIssues').length) {
        var bsCollapse2 = new bootstrap.Collapse('#collapseIssues', {
            show: true
        });
    }
    if ($('#collapseBonuses').length) {
        var bsCollapse2 = new bootstrap.Collapse('#collapseBonuses', {
            show: true
        });
    }

    $("#addIssuesForm").trigger("reset");
}

function SearchBySeries(source) {
    $('#search-field').val(source.textContent);
    filter();
}

function importComplete(data) {

    $("#import-spinner").hide();

    if (data.status === 200) {
        var resp = data.responseJSON;
        $('#NewVolume_SeriesName').val(resp.series);
        $('#NewVolume_Title').val(resp.title);

        if (resp.volumeNumber >= 0) {
            $('#NewVolume_Number').val(resp.volumeNumber);
            $('#typeVolume').prop("checked", true);
        }
        else {
            $('#NewVolume_Number').val(1);
            $('#typeOneShot').prop("checked", true);
        }

        if (resp.authors.length === 0) {
            fillAuthors(resp.series);
        }
        else {
            $('#NewVolume_Authors').val(resp.authors);
        }

        $('#NewVolume_ReleaseDate').val(resp.release);
        handleCoverInput(resp.cover);

        $('#NewVolume_Digitality').val(resp.type).change();
        if ($('#new-PurchaseStatus').val() == "Announced") {
            $('#new-PurchaseStatus').val(resp.status).change();
        }

        $('#NewVolume_PublisherName').val(resp.publisher);
        $('#imported-publisher').show();

        $('#NewVolume_ISBN').val(resp.isbn);
        $('#imported-isbn').show();

        $('#NewVolume_TotalVolumes').val(resp.totalVolumes);
        $('#NewVolume_SeriesStatus').val(resp.seriesStatus);
        $('#NewVolume_SeriesOriginalName').val(resp.originalSeriesName);
    }   
    else {
        console.error(data.responseText);
    }
}


async function handleCoverInput(coverUrl) {

    $('#NewVolume_CoverToDownload').val(coverUrl).change();
    return;




    const designFile = await createFile(coverUrl);

    if (designFile == null)
        return;

    const input = document.querySelector('#NewVolume_CoverFile');
    const dt = new DataTransfer();
    dt.items.add(designFile);
    input.files = dt.files;
    const event = new Event("change", {
        bubbles: !0,
    });
    input.dispatchEvent(event);
}
async function createFile(url) {

    try {
        let response = await fetch(url);

        let data = await response.blob();
        let metadata = {
            type: "image/png",
        };
        var filename = url.split('/').pop()
        return new File([data], filename, metadata);


    } catch (e) {

        $('#NewVolume_CoverToDownload').val(url).change();
        return null;
    }
}

function importBegin(data) {
    $("#import-spinner").show();
}
function updateBegin(data) {
    $("#update-spinner").show();
}

function fillAuthors(source) {
    $("#import-spinner").show();


    if (typeof source === 'string') {
        var seriesName = source
    } else {
        var seriesName = $(source).val();
    }

    $.ajax({
        url: "?handler=SeriesAuthor&series=" + seriesName,
        type: 'GET',
        cache: false
    }).always(function (result) {
        $("#import-spinner").hide();
    }).done(function (result) {
        $('#NewVolume_Authors').val(result);
    });
}