import { apiClient } from './client'

export const costApi = {
  getSummary: async () => {
    const res = await apiClient.get('/cost/summary')
    return res.data
  },
  getByTeam: async () => {
    const res = await apiClient.get('/cost/by-team')
    return res.data
  },
  getByService: async () => {
    const res = await apiClient.get('/cost/by-service')
    return res.data
  },
}
