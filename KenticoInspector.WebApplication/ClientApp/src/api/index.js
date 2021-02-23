import axios from "axios";
import { arrayToObject } from '../helpers'
import { reportService } from './report-service'

export default {
    getInstances() {
        return new Promise((resolve) => {
            axios.get("/api/instances")
                .then(r => r.data)
                .then(instances => {
                    const instancesObject = arrayToObject(instances, "guid")
                    resolve(instancesObject)
                })
        })
    },

    upsertInstance(instance) {
        return new Promise((resolve, reject) => {
            axios.post("/api/instances", instance)
                .then(r => r.data)
                .catch(reject)
                .then(instance => {
                    resolve(instance)
                })
        })
    },

    deleteInstance(guid) {
        return new Promise((resolve) => {
            axios.delete(`/api/instances/${guid}`)
                .then(r => r.data)
                .then(result => {
                    resolve(result)
                })
        })
    },

    getInstanceDetails(guid) {
        return new Promise((resolve, reject) => {
            axios.get(`/api/instances/details/${guid}`)
                .then(r => r.data)
                .catch(reason => {
                    reject({ message: 'Error Connecting', response: reason.response })
                })
                .then(result => {
                    resolve(result)
                })
        })
    },
    getCoreVersion() {
        return new Promise((resolve, reject) => {
            axios.get(`/api/versions/getCoreVersion`)
                .then(r => r.data)
                .catch(reason => {
                    reject({ message: 'Error Connecting', response: reason.response })
                })
                .then(result => {
                    resolve(result)
                })
        })
    },

    reportService
}