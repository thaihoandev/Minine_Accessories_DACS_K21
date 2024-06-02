/* JS Document */

/******************************

[Table of Contents]

1. Vars and Inits
2. Set Header
3. Init Menu
4. Init Quantity


******************************/

$(document).ready(function () {
	$('.navbar-light .dmenu').hover(function () {
		$(this).find('.sm-menu').first().stop(true, true).slideDown(150);
	}, function () {
		$(this).find('.sm-menu').first().stop(true, true).slideUp(105)
	});
});

$(document).ready(function () {
	$(".megamenu").on("click", function (e) {
		e.stopPropagation();
	});
});
