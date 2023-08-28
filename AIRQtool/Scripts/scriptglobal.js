function backdrop(s) {
    var b = $('.layout-backdrop');
    if (s === true) {
        b.attr('aria-hidden', 'false');
        setTimeout(function () { b.addClass('show'); }, 200);
    } else {
        b.removeClass('show');
        setTimeout(function () { b.attr('aria-hidden', 'true'); }, 200);
    }
}

function drawer(s) {
    var b = $('.layout-drawer'), c = $('html').addClass('overflow overflow-drawer');
    if (s === true) {
        b.attr('aria-hidden', 'false');
        setTimeout(function () { b.addClass('show'); c.addClass('overflow overflow-drawer'); }, 200);
        backdrop(true);
    } else {
        b.removeClass('show');
        setTimeout(function () { b.attr('aria-hidden', 'true'); c.removeAttr('class'); }, 200);
        backdrop(false);
    }
}

function modalTabs() {
    $('.modal-tabs').each(function () {
        var modalTabs = $(this);

        modalTabs.find('.modal-tab-content').attr('aria-hidden', 'true').first().attr('aria-hidden', 'false');
        modalTabs.find('.modal-title:first').addClass('active');

        modalTabs.find('.modal-title').click(function () {
            var self = $(this);

            self.addClass('active').siblings().removeClass('active');
            self.parent().siblings('.modal-tab-content').attr('aria-hidden', 'true');

            var activeTab = self.attr('aria-controls');

            $('#' + activeTab).attr('aria-hidden', 'false');

            return false;
        });
    });
}

function modal(s, i = null) {
    var a = $('#' + i + '.layout-modal, #' + i + ' .modal'),
        b = $('html'),
        c = $('.layout-modal, .modal'),
        d = $('#' + i + ' .modal-loading'),
        e = $('#' + i + ' .modal-tabs');

    c.removeClass('show');

    setTimeout(function () {
        if (s === true) {
            a.attr('aria-hidden', 'false');

            setTimeout(function () {
                a.addClass('show');
                b.addClass('overflow overflow-modal');

                if (d.length !== 0) {
                    setTimeout(function () {
                        d.removeClass('show');

                        setTimeout(function () {
                            d.attr('aria-hidden', 'true');
                            d.next().attr('aria-hidden', 'false');
                        }, 200);
                    }, 1000);
                }

                if (e.length !== 0) { modalTabs(); }
            }, 200);

            backdrop(true);

        } else {
            a.removeClass('show');

            setTimeout(function () {
                a.attr('aria-hidden', 'true');
                b.removeAttr('class');

                if (d.length !== 0) {
                    setTimeout(function () {
                        d.addClass('show').attr('aria-hidden', 'false');
                        d.next().attr('aria-hidden', 'true');
                    }, 1000);
                }
            }, 200);

            backdrop(false);
        }
    }, 50);
}

function backdropCloseAll() {
    var b = $('.layout-backdrop');
    b.on('click', function () {
        modal(false);
        drawer(false);
        board(false);
    });
}

function showDrawer() {
    drawer(true);
}

function hideDrawer() {
    drawer(false);
}

function showModal(i) {
    modal(true, i);
}

function hideModal(i) {
    modal(false, i);
}

function board(x, id = null) {
    var a = $('html'), b = $('#' + id), c = $('#' + id + ', #' + id + ' .board');
    if (x === true) {
        a.addClass('overflow overflow-board');
        b.attr('aria-hidden', 'false');

        setTimeout(function () {
            c.addClass('show');
        }, 200);

        backdrop(true);
    } else {
        a.removeClass('overflow overflow-board');
        $('.layout-board, .layout-board .board').removeClass('show');

        setTimeout(function () {
            $('.layout-board').attr('aria-hidden', 'true');
        }, 200);

        backdrop(false);
    }
}

function showBoard(i) {
    board(true, i);
}

function hideBoard(i) {
    board(false, i);
}

backdropCloseAll();

// =========================================================================================================== Simulation scripts
function preloaderSim() {
    setTimeout(function () {
        setTimeout(function () { $('#wizard-loading').attr('aria-hidden', 'true') }, 200);
        $('#wizard-loading').removeClass('show');
    }, 2000);
}

function showDrawerButtons() {
    var productSelected = $('#drawer-filter input:checked').length;

    if (productSelected > 0) {
        $('#drawer-filter .drawer-foot').attr('aria-hidden', 'false');
    } else {
        $('#drawer-filter .drawer-foot').attr('aria-hidden', 'true');
    }
}

function checkfilter() {
    $('#drawer-filter').on('change', function () {
        showDrawerButtons();
    });

    $('#clearfilter').click(function () {
        $('#drawer-filter input').prop('checked', false);
        showDrawerButtons();
    });
}

function showEmptyCartModal(x) {
    var viewSelected = $('#resultCart');

    if (x === false) {
        viewSelected.attr('href', 'javascript:;');
        viewSelected.click(function () {
            $('#empty-cart, #layout-backdrop').attr('aria-hidden', 'false');
            $('html').addClass('overflow overflow-modal');

            setTimeout(function () {
                $('#empty-cart, #layout-backdrop, #empty-cart .modal').addClass('show');
            }, 200);
        });
    } else {
        viewSelected.attr('href', '06-checkout-orders-(clo).html');
    }
}

function showEmptyCartModal(x) {
    var viewSelected = $('#resultCart');

    if (x === false) {
        viewSelected.attr('href', 'javascript:;');
        viewSelected.click(function () {
            $('#empty-cart, #layout-backdrop').attr('aria-hidden', 'false');
            $('html').addClass('overflow overflow-modal');

            setTimeout(function () {
                $('#empty-cart, #layout-backdrop, #empty-cart .modal').addClass('show');
            }, 200);
        });
    } else {
        viewSelected.attr('href', '06-checkout-orders-(clo).html');
    }
}

function selectProduct() {
    var productList = $('#productList');

    function updateCounter() {
        var selectedProduct = $('#productList input:checked').length,
            viewSelected = $('#resultCart');

        if (selectedProduct > 0) {
            viewSelected.find('.badge').addClass('badge-warning').attr('data-count', selectedProduct).text(selectedProduct);
        } else {
            viewSelected.find('.badge').removeClass('badge-warning').attr('data-count', '0').text('0');
        }

        if (selectedProduct === 0) {
            showEmptyCartModal(false);
        } else {
            showEmptyCartModal(true);
        }
    }

    function updateText() {
        productList.find('input').each(function () {
            var self = $(this)
            parent = self.closest('.card-type-product');

            if (self.prop('checked')) {
                self.next().text('Selected');
                parent.addClass('selected');
            } else {
                self.next().text('Choose');
                parent.removeClass('selected');
            }
        });
    }

    showEmptyCartModal(false);

    productList.change(function () {
        updateCounter();
        updateText(); s
    });
}

preloaderSim();
checkfilter();
selectProduct();

function checkPasswordStrength() {
    var meterInput = $('.password-meter').find('input[type="password"]');

    $.strength = function (wrapper, password, bar) {
        var passwordClassStatus = ['is-password-default', 'is-password-worst', 'is-password-bad', 'is-password-weak', 'is-password-good', 'is-password-strong'];
        var passwordAriaText = ['none', 'worst', 'bad', 'weak', 'good', 'strong'];
        var strength = 0;

        if (password.length > 6)
            strength++;

        if (password.match(/[a-z]/) && password.match(/[A-Z]/))
            strength++;

        if (password.match(/\d+/))
            strength++;

        if (password.match(/.[!,@,#,$,%,^,&,*,?,_,~,-,(,)]/))
            strength++;

        if (password.length > 10)
            strength++;

        wrapper.removeClass(passwordClassStatus).addClass(passwordClassStatus[strength]);
        bar.attr('aria-password-strength', passwordAriaText[strength]);
    };

    meterInput.keyup(function () {
        var self = $(this), selfParent = self.closest('.form-group'), selfStatus = self.next();

        $.strength(selfParent, self.val(), selfStatus);

        if (self.val().length > 0) {
            selfStatus.attr('aria-hidden', 'false');
            setTimeout(function () { selfStatus.addClass('show'); }, 200);
        } else {
            selfStatus.removeClass('show');
            setTimeout(function () { selfStatus.attr('aria-hidden', 'true'); }, 200);
        }
    });
}

function togglePasswordView() {
    var toggleIcon = $('.toggle-password'), toggleInput = toggleIcon.parent().find('input');

    toggleIcon.click(function () {
        var iconTog = $(this),
            inputTog = iconTog.parent().find('input');

        iconTog.toggleClass('icon-view');
        iconTog.toggleClass('icon-view-hide');

        inputTog.attr('type', function (index, attr) { return attr === 'text' ? 'password' : 'text'; });

        setTimeout(function () {
            iconTog.addClass('icon-view').removeClass('icon-view-hide');
            inputTog.attr('type', 'password');
        }, 5000);
    });

    toggleInput.keyup(function (e) {
        var self = $(this), icon = self.closest('.form-group').find('.icon');

        e.preventDefault();

        if (self.val() === '')
            icon.attr('aria-hidden', 'true');
        else
            icon.attr('aria-hidden', 'false');
    });
}

function placeholderLabel() {
    var placeholderGroup = $('.placeholder-label'), placeholderInput = placeholderGroup.find('input[type="password"], input[type="text"], textarea');

    placeholderInput.focusin(function (e) {
        e.preventDefault();
        $(this).closest(placeholderGroup).addClass('focused');
    }).focusout(function () {
        $(this).closest(placeholderGroup).removeClass('focused');
    }).keyup(function () {
        var self = $(this), selfGroup = self.closest(placeholderGroup);

        if (self.val() === '')
            selfGroup.removeClass('filled');
        else
            selfGroup.addClass('filled');
    });
}

checkPasswordStrength();
togglePasswordView();
placeholderLabel();