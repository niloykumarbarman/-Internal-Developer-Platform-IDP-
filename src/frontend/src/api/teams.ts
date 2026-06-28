import { apiClient } from './client'
import type { Team } from '../types'

export const teamsApi = {
  getAll: async (): Promise<Team[]> => {
    const res = await apiClient.get('/auth/teams')
    return res.data
  },
  create: async (data: { name: string; lead: string; email: string }) => {
    const res = await apiClient.post('/auth/teams', data)
    return res.data
  },
  // NOTE: getMembers/addMember removed — no corresponding backend endpoint exists yet
  // (AuthController only exposes register, login, POST teams, GET teams).
  // Re-add once a team-members endpoint is implemented on the backend.
}
