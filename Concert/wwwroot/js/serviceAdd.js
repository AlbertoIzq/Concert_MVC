function AddService (url) {
    $.ajax({
        url: url,
        type: 'POST',
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
            }
            else {
                toastr.warning(data.message);
            }
        }
    })
}