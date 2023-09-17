$(function () {



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
        data: { id: id }
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