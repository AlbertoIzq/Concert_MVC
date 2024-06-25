function AddService (url) {
    $.ajax({
        url: url,
        type: 'POST',
        success: function (data) {
            toastr.success(data.message);
        }
    })
}