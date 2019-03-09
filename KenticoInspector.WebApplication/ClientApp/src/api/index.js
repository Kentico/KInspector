import axios from "axios";

export default {
  getInstanceConfigurations () {
    return new Promise((resolve)=>{
      axios.get("/api/instances")
      .then(r => r.data)
      .then(instances => {
        resolve(instances)
      })
    })
  },

  upsertInstanceConfiguration (instanceConfiguration) {
    return new Promise((resolve)=>{
      axios.post("/api/instances", instanceConfiguration)
      .then(r => r.data)
      .then(instances => {
        resolve(instances)
      })
    })
  },

  deleteInstanceConfiguration (instanceConfigurationGuid) {
    return new Promise((resolve)=>{
      console.log(instanceConfigurationGuid)
      axios.delete(`/api/instances/${instanceConfigurationGuid}`)
      .then(() => {
        resolve()
      })
    })
  }
}