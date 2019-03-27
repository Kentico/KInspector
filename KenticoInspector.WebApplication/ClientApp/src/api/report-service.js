import axios from "axios";
import { arrayToObject } from '../helpers'

class ReportService {
  getReports () {
    return new Promise((resolve)=>{
      axios.get("/api/reports")
      .then(r => r.data)
      .then(reports => {
        const reportsObject = arrayToObject(reports, "codename")
        resolve(reportsObject)
      })
    })
  }
}

export const reportService = new ReportService()