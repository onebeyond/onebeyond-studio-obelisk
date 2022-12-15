// Sidebar
var offcanvasToggleBtn = document.querySelector('.sidebar-toggler');
var closeSidebarBtn = document.querySelector('.close-sidebar');
var sidebarLeft = document.querySelector('#sidebar-left');
var sidebarMask = document.querySelector('#sidebar-mask');
var sidebarLink = document.querySelectorAll('.sidebar-menu li a');

function offcanvasToggle() {
    sidebarLeft.classList.toggle('show');
    sidebarMask.classList.toggle('show');
}

offcanvasToggleBtn.onclick = offcanvasToggle;
sidebarMask.onclick = offcanvasToggle;
closeSidebarBtn.onclick = offcanvasToggle;

for (var i = 0; i < sidebarLink.length; i++) {
    sidebarLink[i].addEventListener('click', function (event) {
        offcanvasToggle();
    });
}


// Sidebar search

var sidebarSearchInput, filter, ul, li, sidebarLinkTxt, txtValue, collapse, collapseTitle, separator, btnResetSidebarSearch;


function sidebarSearch() {
    sidebarSearchInput = document.getElementById('sidebarSearchInput');
    filter = sidebarSearchInput.value.toUpperCase();
    ul = document.getElementById("sidebar-left");
    li = ul.querySelectorAll(".nav-item");
    collapse = ul.querySelectorAll(".collapse");
    collapseTitle = ul.querySelectorAll(".nav-section-title");
    separator = ul.querySelectorAll(".separator");
    btnResetSidebarSearch = document.getElementById("reset-sidebar-search");


    if (sidebarSearchInput.value.length > 0) {
        btnResetSidebarSearch.classList.add("search-mode-show");
        showOnSearch(collapse);
        hideOnSearch(separator);
        hideOnSearch(collapseTitle);
    } else {
        sidebarSearchReset();
    }

    // Loop through all list items, and hide those who don't match the search query
    for (i = 0; i < li.length; i++) {
        sidebarLinkTxt = li[i].getElementsByTagName("a")[0];
        txtValue = sidebarLinkTxt.textContent || sidebarLinkTxt.innerText;

        sidebarLinkTxt.addEventListener('click', function (event) {
            sidebarSearchReset();
        });

        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            li[i].classList.remove("search-mode-hide");
            li[i].classList.add("search-mode-show");
        } else {
            li[i].classList.remove("search-mode-show");
            li[i].classList.add("search-mode-hide");
        }
    }
}


function sidebarSearchReset() {
    btnResetSidebarSearch.classList.remove("search-mode-show");
    resetOnSearch(li);
    resetOnSearch(collapse);
    resetOnSearch(separator);
    resetOnSearch(collapseTitle);
    sidebarSearchInput.value = '';
}

function hideOnSearch(el) {
    for (i = 0; i < el.length; i++) {
        el[i].classList.remove("search-mode-show");
        el[i].classList.add("search-mode-hide");
    }
}

function showOnSearch(el) {
    for (i = 0; i < el.length; i++) {
        el[i].classList.remove("search-mode-hide");
        el[i].classList.add("search-mode-show");
    }
}

function resetOnSearch(el) {
    for (i = 0; i < el.length; i++) {
        el[i].classList.remove("search-mode-hide");
        el[i].classList.remove("search-mode-show");
    }
}

// Title bar on scroll
if ($('.title-bar')[0]) {
    var divTop = $('.title-bar').offset().top;
    var titleBarHeight = $('.title-bar').outerHeight();
}
var navbarHeight = $('#main-menu').outerHeight();

$(window).resize(function () {
    titleBarHeight = $('.title-bar').outerHeight();
    navbarHeight = $('#main-menu').outerHeight();
});

function append(position) {
    if (position == top) {
        $('.page-content').css('padding-top', titleBarHeight + 30);
    }
    else {
        $('.page-content').removeAttr("style");
    }
}


// Close mobile menu once link tapped
$('.navbar-nav li a').on('click', function () {
    if ($(this).hasClass("dropdown-toggle")) {
        // do nothing
    }
    else {
        $('#navbarSupportedContent').removeClass('show');
        $('.hamburger-icon').removeClass('hamburger-icon-close');
    }
});