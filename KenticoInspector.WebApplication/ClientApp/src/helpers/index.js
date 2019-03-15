export function arrayToObject(array, keyField) {
  return array.reduce((obj, item) =>{
    obj[item[keyField]] = item
    return obj
  }, {})
}
