import toastr from 'toastr'

export default {
  success (message) {
    toastr.success(message)
  },
  error (title, errors) {
    toastr.error(title)
  }
}
