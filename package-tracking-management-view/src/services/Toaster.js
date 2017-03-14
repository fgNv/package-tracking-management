import toastr from 'toastr'

export default {
  success (message) {
    toastr.success(message)
  },
  error (title, errors) {
    var content = '<h3>' + title + '</h3>'

    if (errors) {
      content += '<ul>' + errors.reduce((acc, i) => '<li>' + i + '</li>', '') + '</ul>'
    }
    toastr.error(content)
  }
}
