import { apiClient } from './client'

export const observabilityApi = {
  getMetrics: async () => {
    const res = await apiClient.get('/observability/metrics')
    return res.data
  },
  getAlerts: async () => {
    const res = await apiClient.get('/observability/alerts')
    return res.data
  },
  getLogs: async (service: string) => {
    const res = await apiClient.get(`/observability/logs?service=${service}`)
    return res.data
  },
}
