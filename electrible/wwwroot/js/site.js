// Function to update cart item count
function updateCartItemCount() {
    $.ajax({
        url: '/Cart/GetCartItemCount',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var itemCount = data.itemCount;
            $('.cart-item-count').text(itemCount);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching cart item count:', error);
        }
    });
}

// Call the function to update cart count on page load
$(document).ready(function () {
    updateCartItemCount();
});