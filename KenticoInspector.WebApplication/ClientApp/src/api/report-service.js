import * as reportsData from './sample-data/reports.json'

class ReportService {
  async getReports () {
    return reportsData.default
  }
}

export const reportService = new ReportService()