import { apiClient } from './client'

export const incidentsApi = {
  getAll: async () => {
    const res = await apiClient.get('/incidents')
    return res.data
  },
  create: async (data: { title: string; severity: string; description: string }) => {
    const res = await apiClient.post('/incidents', data)
    return res.data
  },
  resolve: async (id: string) => {
    const res = await apiClient.post(`/incidents/${id}/resolve`)
    return res.data
  },
  getTimeline: async (id: string) => {
    const res = await apiClient.get(`/incidents/${id}/timeline`)
    return res.data
  },
}
