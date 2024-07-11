function ConfirmOrder(orderId) {
    Swal.fire({
        title: "Are you sure concert date and address are ok?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        console.log("hey");
        if (result.isConfirmed) {
            var url = '/Admin/Order/Confirm?orderId=' + orderId;
            $.ajax({
                url: url,
                type: 'POST'
            })
            console.log("wazuuuup");
        }
    });
}