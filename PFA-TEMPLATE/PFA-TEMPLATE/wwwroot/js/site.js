document.addEventListener("DOMContentLoaded", function () {
    // Get current page URL
    const currentLocation = window.location.href;

    // Get all navigation links
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');

    // Loop through each link
    navLinks.forEach(link => {
        // Default state - remove active class from all links
        link.classList.remove('active');

        // Check if this is an ASP.NET Core link with asp-controller
        const controller = link.getAttribute('asp-controller');
        const action = link.getAttribute('asp-action');

        if (controller && action) {
            // For ASP.NET Core links, check URL for controller and action
            if (currentLocation.includes(`/${controller}/`) && currentLocation.includes(`/${action}`)) {
                link.classList.add('active');
            }
        } else {
            // For regular href links
            const linkHref = link.getAttribute('href');

            // If it's not a "#" placeholder link and the URL includes this link path
            if (linkHref && linkHref !== '#' && currentLocation.includes(linkHref)) {
                link.classList.add('active');
            }

            // Special case for home/dashboard
            if (linkHref && linkHref.includes('dashboard') &&
                (currentLocation.endsWith('/') || currentLocation.endsWith('/home'))) {
                link.classList.add('active');
            }
        }
    });
});


document.addEventListener('DOMContentLoaded', function () {
    const sidebarContent = document.querySelector('.sidebar-content');
    const scrollIndicator = document.querySelector('.scroll-indicator');

    sidebarContent.addEventListener('scroll', function () {
        const scrollPercentage = (sidebarContent.scrollTop / (sidebarContent.scrollHeight - sidebarContent.clientHeight)) * 100;
        const indicatorPosition = (scrollPercentage / 100) * (sidebarContent.clientHeight - scrollIndicator.clientHeight);

        scrollIndicator.style.top = indicatorPosition + 'px';

        // Fade effect based on scroll position
        if (scrollPercentage > 0) {
            scrollIndicator.style.opacity = '0.8';
        } else {
            scrollIndicator.style.opacity = '0.4';
        }
    });

    // Hide indicator after inactivity
    let timeout;
    sidebarContent.addEventListener('scroll', function () {
        scrollIndicator.style.opacity = '0.8';
        clearTimeout(timeout);
        timeout = setTimeout(function () {
            scrollIndicator.style.opacity = '0.4';
        }, 1500);
    });
});

