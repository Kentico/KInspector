import axios from "axios";

class ActionService {
    getAll(instanceGuid) {
    return new Promise((resolve)=>{
      axios.get(`/api/actions/${instanceGuid}`)
      .then(r => r.data)
      .then(reports => {
        resolve(reports)
      })
    })
  }

  execute ({codename, instanceGuid, options}) {
    return new Promise((resolve)=>{
      axios.post(`/api/actions/${codename}/execute/${instanceGuid}`, options)
      .then(r => r.data)
      .then(results => {
        resolve(results)
      })
    })
  }
}

export const actionService = new ActionService()