import { apiClient } from './client'

export const teamsApi = {
  getAll: async () => {
    const res = await apiClient.get('/teams')
    return res.data
  },
  create: async (data: { name: string; lead: string; email: string }) => {
    const res = await apiClient.post('/teams', data)
    return res.data
  },
  getMembers: async (id: string) => {
    const res = await apiClient.get(`/teams/${id}/members`)
    return res.data
  },
  addMember: async (teamId: string, userId: string) => {
    const res = await apiClient.post(`/teams/${teamId}/members`, { userId })
    return res.data
  },
}
