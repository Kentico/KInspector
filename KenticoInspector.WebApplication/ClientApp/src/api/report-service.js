import axios from "axios";

class ReportService {
  getReports () {
    return new Promise((resolve)=>{
      axios.get("/api/reports")
      .then(r => r.data)
      .then(reports => {
        resolve(reports)
      })
    })
  }

  getReportResults ({codename, instanceGuid}) {
    return new Promise((resolve)=>{
      axios.get(`/api/reports/${codename}/results/${instanceGuid}`)
      .then(r => r.data)
      .then(results => {
        resolve(results)
      })
    })
  }
}

export const reportService = new ReportService()