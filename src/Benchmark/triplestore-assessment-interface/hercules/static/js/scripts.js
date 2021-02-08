setUserWeights = function(reset) {
	sessionStorage.setItem('herc:weights-custom', ''+!reset);
    $('input.herc_weight_setter').each(function() {
        var vstor = sessionStorage.getItem($(this).prop('name'));
        $(this).val(reset || vstor === null ? $(this).data('defaultvalue') : vstor).change()
    })
}