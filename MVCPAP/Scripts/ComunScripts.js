$(function () {
    $(".VideoAuthor").click(function (f) {
        window.location.assign("../../../Home/profile/" + f.toElement.id)
    });

    $(".videotitle").click(function (f) {
        window.location.assign("../../../Home/Video/" + f.toElement.id.split('_')[1]);
    });
    $(".thumbnail").click(function (f) {
        window.location.assign("../../../Home/Video/" + f.toElement.id.split('_')[1]);
    });
    $(document).ready(function () {
        $("#Search").focus();
        $("#loading").hide();
    });


    $('body').click(function (evt) {
        if (evt.target.id.startsWith("buttonOptions_"))
            return;
        if ($(evt.target).closest('.buttonOptions').length)
            return;

        if (evt.target.id.startsWith("divOptions_"))
            return;
        if ($(evt.target).closest('.commentOptions').length)
            return;

        $(".commentOptions").hide();
    });

    $("#myForm").ready(function () {
        $("#AddCommentDiv").hide();
        //$(".commentOptions").hide();
        //$(".LabelComment").hide();
        $("#AddCommentError").hide();
    });

    $("#cancelComment").click(function () {
        $("#AddCommentDiv").hide();
        $("#buttonAddComment").show();
    });

    function keypressHandler(e) {
        if (e.which == 13) {
            if ($("#AddCommentDiv").is(":visible")) {
                e.preventDefault();
                $(this).blur();
                $('#buttonAddCommentSend').focus().click();
            }
        }
    }

    $('#myForm').keypress(keypressHandler);

    //$(".buttonOptions").click(function (b) {
    //    var id = b.toElement.id.split('_')[1];
    //    var WasVisible = $("#divOptions_" + id).is(":visible");
    //    $(".commentOptions").hide();
    //    $("#divOptions_" + id).toggle();
    //    if (WasVisible)
    //        $("#divOptions_" + id).hide();
    //});

    $(".DeleteComment").click(function (p) {
        var id = p.toElement.id.split('_')[1];

        $.ajax({
            url: "../../../Home/DeleteComment/" + id,
            type: "POST",
            error: function (response) {
                console.log("Error here :"+response);
            },
            success: function (response) {
                console.log(response);
            }
        });
        $(".commentOptions").hide();

    });


    $(".buttonUpvote").click(function (img) {
        var id = img.toElement.id.split('_')[1];
        $.ajax({
            url: "../../../Home/UpvoteComment/" + id,
            type: "GET",
            error: function (response) {
                console.log(response);
            },
            success: function (response) {
                console.log(response);
            }
        });


    });


});