$(function () {
    // Handler for opening the modal
    $('.add-to-playlist-button').click(function (event) {
        event.preventDefault();

        // Get the track ID and name from the table row
        var trackId = $(this).data('track-id');

        // Send an AJAX request to get the playlists
        $.get('/Playlist/GetPlaylists', { trackId: trackId }, function (data) {
            // Add the playlists as checkboxes to the modal
            $('#playlists').empty();
            for (var i = 0; i < data.length; i++) {
                var playlist = data[i];
                var checkboxHtml = '<div class="form-check"><input class="form-check-input" type="checkbox" id="playlist-' + playlist.id + '" name="playlistIds" value="' + playlist.id + '"';
                if (playlist.checkedPlaylist) {
                    checkboxHtml += ' checked';
                }
                checkboxHtml += '><label class="form-check-label" for="playlist-' + playlist.id + '">' + playlist.name + '</label></div>';
                $('#playlists').append(checkboxHtml);
            }

            // Set the track ID and name as data attributes on the "Save" button
            $('#savePlaylistButton').data('track-id', trackId);

            // Show the modal
            $('#addToPlaylistModal').modal('show');

            // Add click handler for the "Close" button
            $('#addToPlaylistModal .close').click(function () {
                $('#addToPlaylistModal').modal('hide');
            });

            // Add click handler for the "Cancel" button
            $('#addToPlaylistModal .btn-secondary').click(function () {
                $('#addToPlaylistModal').modal('hide');
            });
        });
    });

    $("#savePlaylistButton").click(function () {
        var selectedPlaylists = [];
        $('input[type=checkbox]:checked').each(function () {
            selectedPlaylists.push($(this).val());
        });

        var trackId = $('#savePlaylistButton').data('track-id');

        $.ajax({
            url: '/Track/AddTrackToPlaylist',
            type: 'POST',
            traditional: true,
            data: { playlistIds: selectedPlaylists, trackId: trackId },
            success: function () {
                $("#addToPlaylistModal").modal('hide')
                alert('Трек успешно добавлен в плейлисты');
            },
            error: function () {
                alert('Произошла ошибка при добавлении трека в плейлисты');
            }
        });
    });
});