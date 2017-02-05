import Moment from 'moment'

exports.install = function (Vue) {
  Vue.filter('moment', function (date) {
    if (!date) {
      return 'N/A'
    }
    return Moment(date).format('DD/MM/YYYY HH:mm')
  })
}
