const makeFormatPrice = (element) => {
    const price = parseInt(element.textContent);
    const newPrice = price.toLocaleString('it-IT', { style: 'currency', currency: 'VND' })
    element.textContent = newPrice
}

const prices = document.querySelectorAll('.priceFormat')
if (prices) {
    prices.forEach(item => {
        makeFormatPrice(item)
    })
}

const redirectToUserDetail = () => {
    return location.assign('/user/detail');
}

const submitForm = (buttonQuery, formQuery) => {
    $(buttonQuery).on('click', () => $(formQuery).submit())
}

const formatOrderStatus = () => {
    const statuses = document.querySelectorAll('.orderStatus');
    if (statuses) {
        statuses.forEach((s) => {
            switch (s.innerText) {
                case 'InProgress':
                    s.innerText = 'Đang xử lý'
                    s.setAttribute('class', 'text-info')
                    break;
                case 'Shipping':
                    s.innerText = 'Đang giao hàng'
                    s.setAttribute('class', 'text-info')
                    break;
                case 'Success':
                    s.setAttribute('class', 'text-success')
                    s.innerText = 'Hoàn thành'
                    break;
                case 'Confirmed':
                    s.setAttribute('class', 'text-info')
                    s.innerText = 'Đã xác nhận'
                    break;
                case 'Canceled':
                    s.setAttribute('class', 'text-danger')
                    s.innerText = 'Hủy bỏ'
                    break;
            }

        })
    }
}

formatOrderStatus();

const openModal = (modalId, submitId, data) => {
    modal = document.querySelector(`#${modalId}`);
    if (modal) {
        modal.style.display = 'flex';
        const submitBtn = document.querySelector(`#${submitId}`);
        if (submitBtn && data) {
            submitBtn.setAttribute('data', data);
        }
    }
}



const handleOnCancel = () => {
    document.querySelector('.co-modal').style.display = 'none';
    document.querySelector('.co-submit-btn').removeAttribute('data');
}

const squareShapes = document.querySelectorAll('.squareShape');

if (squareShapes) {
    squareShapes.forEach((item) => {
        const itemHeight = item.clientWidth;
        item.style.height = `${itemHeight}px`;
    })
}


const redirectToProductDetail = (id) => {
    location.assign(`/product/${id}`);
}


function handleAddToCart(product) {

    const productCart = JSON.stringify(product);
    event.preventDefault();

    $.ajax({
        type: 'POST',
        data: productCart,
        dataType: 'json',
        contentType: 'application/json;',
        url: '/Cart/AddToCart',
        success: async (response) => {
            $('#notificationMsg').removeClass("d-none").addClass('d-block').html("Thêm sản phẩm vào giỏ thành công!")
            $('#cartQuantity').html(response?.products?.length ?? 0)
            await new Promise(resolve => setTimeout(() => {
                $('#notificationMsg').fadeOut('slow');
                resolve();
            }, 1500))
            $('#notificationMsg').addClass('d-none').removeClass('d-block')
        },
        error: (response) => {
            console.log(response, 'voibenho')
        }
    })
}