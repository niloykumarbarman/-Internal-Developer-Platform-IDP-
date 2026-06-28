import { useState } from 'react'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Users, Plus, Search, Mail, Shield, GitBranch, Settings } from 'lucide-react'

const mockTeams = [
  { id: 1, name: 'Platform Engineering', members: 8, lead: 'Alice Johnson', services: 12, role: 'Admin', email: 'platform@company.com', color: 'bg-blue-500' },
  { id: 2, name: 'Backend Services', members: 12, lead: 'Bob Smith', services: 24, role: 'Developer', email: 'backend@company.com', color: 'bg-green-500' },
  { id: 3, name: 'Frontend Team', members: 6, lead: 'Carol White', services: 8, role: 'Developer', email: 'frontend@company.com', color: 'bg-purple-500' },
  { id: 4, name: 'Data Engineering', members: 5, lead: 'David Lee', services: 6, role: 'Developer', email: 'data@company.com', color: 'bg-orange-500' },
  { id: 5, name: 'Security Team', members: 4, lead: 'Eve Davis', services: 3, role: 'Platform Engineer', email: 'security@company.com', color: 'bg-red-500' },
  { id: 6, name: 'ML Platform', members: 7, lead: 'Frank Zhang', services: 9, role: 'Developer', email: 'ml@company.com', color: 'bg-teal-500' },
]

const mockMembers = [
  { id: 1, name: 'Alice Johnson', role: 'Admin', team: 'Platform Engineering', avatar: 'AJ', status: 'active' },
  { id: 2, name: 'Bob Smith', role: 'Developer', team: 'Backend Services', avatar: 'BS', status: 'active' },
  { id: 3, name: 'Carol White', role: 'Developer', team: 'Frontend Team', avatar: 'CW', status: 'active' },
  { id: 4, name: 'David Lee', role: 'Developer', team: 'Data Engineering', avatar: 'DL', status: 'inactive' },
  { id: 5, name: 'Eve Davis', role: 'Platform Engineer', team: 'Security Team', avatar: 'ED', status: 'active' },
]

export default function TeamsPage() {
  const [activeTab, setActiveTab] = useState<'teams' | 'members'>('teams')
  const [search, setSearch] = useState('')

  const filteredTeams = mockTeams.filter(t =>
    t.name.toLowerCase().includes(search.toLowerCase())
  )
  const filteredMembers = mockMembers.filter(m =>
    m.name.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-white">Team Management</h1>
          <p className="text-slate-400 mt-1">Manage teams, members, and permissions</p>
        </div>
        <Button className="bg-blue-600 hover:bg-blue-700">
          <Plus className="w-4 h-4 mr-2" /> Create Team
        </Button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {[
          { label: 'Total Teams', value: mockTeams.length, icon: Users, color: 'text-blue-400' },
          { label: 'Total Members', value: 42, icon: Users, color: 'text-green-400' },
          { label: 'Active Services', value: 62, icon: GitBranch, color: 'text-purple-400' },
          { label: 'Admins', value: 3, icon: Shield, color: 'text-orange-400' },
        ].map((stat) => (
          <Card key={stat.label} className="bg-slate-800 border-slate-700">
            <CardContent className="p-4 flex items-center gap-3">
              <stat.icon className={`w-8 h-8 ${stat.color}`} />
              <div>
                <p className="text-2xl font-bold text-white">{stat.value}</p>
                <p className="text-slate-400 text-sm">{stat.label}</p>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Tabs */}
      <div className="flex gap-2">
        {(['teams', 'members'] as const).map(tab => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={`px-4 py-2 rounded-lg capitalize font-medium transition-colors ${
              activeTab === tab ? 'bg-blue-600 text-white' : 'text-slate-400 hover:text-white'
            }`}
          >
            {tab}
          </button>
        ))}
      </div>

      {/* Search */}
      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
        <Input
          placeholder={`Search ${activeTab}...`}
          value={search}
          onChange={(e: React.ChangeEvent<HTMLInputElement>) => setSearch(e.target.value)}
          className="pl-10 bg-slate-800 border-slate-700 text-white"
        />
      </div>

      {/* Teams Grid */}
      {activeTab === 'teams' && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filteredTeams.map(team => (
            <Card key={team.id} className="bg-slate-800 border-slate-700 hover:border-blue-500 transition-colors">
              <CardHeader className="pb-3">
                <div className="flex items-center gap-3">
                  <div className={`w-10 h-10 rounded-lg ${team.color} flex items-center justify-center`}>
                    <Users className="w-5 h-5 text-white" />
                  </div>
                  <div>
                    <CardTitle className="text-white text-base">{team.name}</CardTitle>
                    <p className="text-slate-400 text-sm">Lead: {team.lead}</p>
                  </div>
                </div>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="flex justify-between text-sm">
                  <span className="text-slate-400">Members</span>
                  <span className="text-white font-medium">{team.members}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-slate-400">Services</span>
                  <span className="text-white font-medium">{team.services}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-slate-400">Role</span>
                  <Badge variant="outline" className="border-blue-500 text-blue-400 text-xs">{team.role}</Badge>
                </div>
                <div className="flex items-center gap-1 text-slate-400 text-xs">
                  <Mail className="w-3 h-3" />
                  {team.email}
                </div>
                <div className="flex gap-2 pt-2">
                  <Button size="sm" variant="outline" className="flex-1 border-slate-600 text-slate-300 hover:bg-slate-700 text-xs">
                    <Settings className="w-3 h-3 mr-1" /> Manage
                  </Button>
                  <Button size="sm" variant="outline" className="flex-1 border-slate-600 text-slate-300 hover:bg-slate-700 text-xs">
                    <Users className="w-3 h-3 mr-1" /> Members
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* Members List */}
      {activeTab === 'members' && (
        <Card className="bg-slate-800 border-slate-700">
          <CardContent className="p-0">
            <table className="w-full">
              <thead>
                <tr className="border-b border-slate-700">
                  <th className="text-left p-4 text-slate-400 font-medium">Member</th>
                  <th className="text-left p-4 text-slate-400 font-medium">Role</th>
                  <th className="text-left p-4 text-slate-400 font-medium">Team</th>
                  <th className="text-left p-4 text-slate-400 font-medium">Status</th>
                  <th className="text-left p-4 text-slate-400 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredMembers.map(member => (
                  <tr key={member.id} className="border-b border-slate-700/50 hover:bg-slate-700/30">
                    <td className="p-4">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-full bg-blue-600 flex items-center justify-center text-white text-xs font-bold">
                          {member.avatar}
                        </div>
                        <span className="text-white">{member.name}</span>
                      </div>
                    </td>
                    <td className="p-4">
                      <Badge variant="outline" className="border-purple-500 text-purple-400 text-xs">{member.role}</Badge>
                    </td>
                    <td className="p-4 text-slate-300">{member.team}</td>
                    <td className="p-4">
                      <Badge className={member.status === 'active' ? 'bg-green-500/20 text-green-400' : 'bg-slate-500/20 text-slate-400'}>
                        {member.status}
                      </Badge>
                    </td>
                    <td className="p-4">
                      <Button size="sm" variant="ghost" className="text-slate-400 hover:text-white h-7 text-xs">
                        Edit
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
