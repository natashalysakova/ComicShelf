function changeAuthorComplete(e) {

    var elements = $(this).find('.update-on-status-change');


    if (e.status == 200) {
        BlinkStatus(elements, 'text-success');
    }
    else {
        BlinkStatus(elements, 'text-danger');
    }
}

function BlinkStatus(elements, className) {
    elements.each(function () {
        // Add the "text-success" class with a delay
        $(this).addClass(className);
        var currentElement = $(this);
        // Remove the "text-success" class after a delay
        setTimeout(function () {
            currentElement.removeClass(className);
        }, 1000);
    });
}