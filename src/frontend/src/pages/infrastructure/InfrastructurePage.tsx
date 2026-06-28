import { useState } from 'react'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Server, Cloud, Database, Plus, RefreshCw, CheckCircle, AlertCircle, Clock, Cpu, HardDrive, Activity } from 'lucide-react'

const mockEnvironments = [
  { id: 1, name: 'Production', provider: 'AWS', region: 'us-east-1', status: 'healthy', nodes: 12, cost: '$2,450/mo', k8sVersion: '1.29' },
  { id: 2, name: 'Staging', provider: 'AWS', region: 'us-west-2', status: 'healthy', nodes: 6, cost: '$980/mo', k8sVersion: '1.29' },
  { id: 3, name: 'Development', provider: 'GCP', region: 'us-central1', status: 'warning', nodes: 3, cost: '$320/mo', k8sVersion: '1.28' },
]

const mockResources = [
  { id: 1, name: 'eks-prod-cluster', type: 'EKS Cluster', env: 'Production', status: 'running', cpu: '68%', memory: '72%' },
  { id: 2, name: 'rds-prod-postgres', type: 'RDS PostgreSQL', env: 'Production', status: 'running', cpu: '34%', memory: '58%' },
  { id: 3, name: 'elasticache-prod', type: 'ElastiCache Redis', env: 'Production', status: 'running', cpu: '22%', memory: '45%' },
  { id: 4, name: 'eks-staging-cluster', type: 'EKS Cluster', env: 'Staging', status: 'running', cpu: '41%', memory: '55%' },
  { id: 5, name: 'rds-staging-postgres', type: 'RDS PostgreSQL', env: 'Staging', status: 'running', cpu: '18%', memory: '32%' },
  { id: 6, name: 'gke-dev-cluster', type: 'GKE Cluster', env: 'Development', status: 'warning', cpu: '85%', memory: '88%' },
]

const mockTerraformModules = [
  { name: 'vpc-module', version: 'v2.1.0', status: 'applied', lastRun: '2h ago' },
  { name: 'eks-cluster', version: 'v3.0.1', status: 'applied', lastRun: '4h ago' },
  { name: 'rds-postgres', version: 'v1.5.2', status: 'applied', lastRun: '1d ago' },
  { name: 'redis-cache', version: 'v1.2.0', status: 'pending', lastRun: '3d ago' },
  { name: 'monitoring-stack', version: 'v2.0.0', status: 'failed', lastRun: '1h ago' },
]

const statusColor: Record<string, string> = {
  healthy: 'text-green-400',
  warning: 'text-yellow-400',
  error: 'text-red-400',
  running: 'text-green-400',
  applied: 'text-green-400',
  pending: 'text-yellow-400',
  failed: 'text-red-400',
}

const statusBg: Record<string, string> = {
  healthy: 'bg-green-500/20 text-green-400',
  warning: 'bg-yellow-500/20 text-yellow-400',
  error: 'bg-red-500/20 text-red-400',
  running: 'bg-green-500/20 text-green-400',
  applied: 'bg-green-500/20 text-green-400',
  pending: 'bg-yellow-500/20 text-yellow-400',
  failed: 'bg-red-500/20 text-red-400',
}

export default function InfrastructurePage() {
  const [activeTab, setActiveTab] = useState<'environments' | 'resources' | 'terraform'>('environments')

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-white">Infrastructure</h1>
          <p className="text-slate-400 mt-1">Manage cloud infrastructure and Terraform resources</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" className="border-slate-600 text-slate-300 hover:bg-slate-700">
            <RefreshCw className="w-4 h-4 mr-2" /> Refresh
          </Button>
          <Button className="bg-blue-600 hover:bg-blue-700">
            <Plus className="w-4 h-4 mr-2" /> Provision
          </Button>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {[
          { label: 'Total Environments', value: 3, icon: Cloud, color: 'text-blue-400' },
          { label: 'Total Resources', value: 18, icon: Server, color: 'text-green-400' },
          { label: 'Monthly Cost', value: '$3,750', icon: Activity, color: 'text-purple-400' },
          { label: 'Terraform Modules', value: 5, icon: HardDrive, color: 'text-orange-400' },
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
        {(['environments', 'resources', 'terraform'] as const).map(tab => (
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

      {/* Environments */}
      {activeTab === 'environments' && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {mockEnvironments.map(env => (
            <Card key={env.id} className="bg-slate-800 border-slate-700 hover:border-blue-500 transition-colors">
              <CardHeader className="pb-3">
                <div className="flex items-center justify-between">
                  <CardTitle className="text-white">{env.name}</CardTitle>
                  <Badge className={statusBg[env.status]}>{env.status}</Badge>
                </div>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="flex justify-between text-sm">
                  <span className="text-slate-400">Provider</span>
                  <span className="text-white font-medium">{env.provider}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-slate-400">Region</span>
                  <span className="text-white">{env.region}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-slate-400">Nodes</span>
                  <span className="text-white">{env.nodes}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-slate-400">K8s Version</span>
                  <span className="text-white">{env.k8sVersion}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-slate-400">Cost</span>
                  <span className="text-green-400 font-medium">{env.cost}</span>
                </div>
                <div className="flex gap-2 pt-2">
                  <Button size="sm" variant="outline" className="flex-1 border-slate-600 text-slate-300 hover:bg-slate-700 text-xs">
                    View Details
                  </Button>
                  <Button size="sm" variant="outline" className="flex-1 border-slate-600 text-slate-300 hover:bg-slate-700 text-xs">
                    Manage
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* Resources */}
      {activeTab === 'resources' && (
        <Card className="bg-slate-800 border-slate-700">
          <CardContent className="p-0">
            <table className="w-full">
              <thead>
                <tr className="border-b border-slate-700">
                  <th className="text-left p-4 text-slate-400 font-medium">Resource</th>
                  <th className="text-left p-4 text-slate-400 font-medium">Type</th>
                  <th className="text-left p-4 text-slate-400 font-medium">Environment</th>
                  <th className="text-left p-4 text-slate-400 font-medium">Status</th>
                  <th className="text-left p-4 text-slate-400 font-medium">CPU</th>
                  <th className="text-left p-4 text-slate-400 font-medium">Memory</th>
                </tr>
              </thead>
              <tbody>
                {mockResources.map(res => (
                  <tr key={res.id} className="border-b border-slate-700/50 hover:bg-slate-700/30">
                    <td className="p-4">
                      <div className="flex items-center gap-2">
                        <Server className="w-4 h-4 text-blue-400" />
                        <span className="text-white text-sm">{res.name}</span>
                      </div>
                    </td>
                    <td className="p-4 text-slate-300 text-sm">{res.type}</td>
                    <td className="p-4">
                      <Badge variant="outline" className="border-slate-600 text-slate-300 text-xs">{res.env}</Badge>
                    </td>
                    <td className="p-4">
                      <Badge className={statusBg[res.status]}>{res.status}</Badge>
                    </td>
                    <td className="p-4">
                      <div className="flex items-center gap-2">
                        <div className="w-16 bg-slate-700 rounded-full h-1.5">
                          <div className={`h-1.5 rounded-full ${parseInt(res.cpu) > 80 ? 'bg-red-500' : parseInt(res.cpu) > 60 ? 'bg-yellow-500' : 'bg-green-500'}`} style={{ width: res.cpu }} />
                        </div>
                        <span className={`text-xs ${statusColor[res.status]}`}>{res.cpu}</span>
                      </div>
                    </td>
                    <td className="p-4">
                      <div className="flex items-center gap-2">
                        <div className="w-16 bg-slate-700 rounded-full h-1.5">
                          <div className={`h-1.5 rounded-full ${parseInt(res.memory) > 80 ? 'bg-red-500' : parseInt(res.memory) > 60 ? 'bg-yellow-500' : 'bg-green-500'}`} style={{ width: res.memory }} />
                        </div>
                        <span className="text-xs text-slate-400">{res.memory}</span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </CardContent>
        </Card>
      )}

      {/* Terraform */}
      {activeTab === 'terraform' && (
        <div className="space-y-4">
          <Card className="bg-slate-800 border-slate-700">
            <CardHeader>
              <CardTitle className="text-white text-lg">Terraform Modules</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              {mockTerraformModules.map((mod, i) => (
                <div key={i} className="flex items-center justify-between p-3 bg-slate-900 rounded-lg">
                  <div className="flex items-center gap-3">
                    {mod.status === 'applied' ? <CheckCircle className="w-4 h-4 text-green-400" /> :
                     mod.status === 'pending' ? <Clock className="w-4 h-4 text-yellow-400" /> :
                     <AlertCircle className="w-4 h-4 text-red-400" />}
                    <div>
                      <p className="text-white text-sm font-medium">{mod.name}</p>
                      <p className="text-slate-400 text-xs">{mod.version} • Last run: {mod.lastRun}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <Badge className={statusBg[mod.status]}>{mod.status}</Badge>
                    <Button size="sm" variant="outline" className="border-slate-600 text-slate-300 hover:bg-slate-700 text-xs h-7">
                      {mod.status === 'failed' ? 'Retry' : 'Plan'}
                    </Button>
                  </div>
                </div>
              ))}
            </CardContent>
          </Card>
        </div>
      )}
    </div>
  )
}
