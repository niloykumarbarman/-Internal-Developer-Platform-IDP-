import {
  Activity, Layers, GitBranch, Shield, AlertTriangle,
  DollarSign, TrendingUp, TrendingDown, Zap, Server,
  Eye, RefreshCw, ArrowUpRight, Clock, CheckCircle2,
  XCircle, Loader2, Minus
} from 'lucide-react'

const statCards = [
  {
    label: 'Total Services', value: '12', delta: '+2 this week', deltaUp: true,
    icon: Layers, grad: 'from-blue-500 to-blue-600', glow: '#3b82f6',
    bg: 'from-blue-500/8 to-transparent',
  },
  {
    label: 'Active Pipelines', value: '4', delta: '2 running now', deltaUp: true,
    icon: Zap, grad: 'from-violet-500 to-violet-600', glow: '#8b5cf6',
    bg: 'from-violet-500/8 to-transparent',
  },
  {
    label: 'K8s Namespaces', value: '8', delta: 'dev · staging · prod', deltaUp: null,
    icon: Server, grad: 'from-cyan-500 to-cyan-600', glow: '#06b6d4',
    bg: 'from-cyan-500/8 to-transparent',
  },
  {
    label: 'Open Incidents', value: '2', delta: '1 critical', deltaUp: false,
    icon: AlertTriangle, grad: 'from-red-500 to-red-600', glow: '#ef4444',
    bg: 'from-red-500/8 to-transparent',
  },
  {
    label: 'Vault Secrets', value: '47', delta: '3 expire soon', deltaUp: false,
    icon: Shield, grad: 'from-amber-500 to-amber-600', glow: '#f59e0b',
    bg: 'from-amber-500/8 to-transparent',
  },
  {
    label: 'Monthly Cost', value: '$2,840', delta: '↓ 8% vs last month', deltaUp: true,
    icon: DollarSign, grad: 'from-emerald-500 to-emerald-600', glow: '#10b981',
    bg: 'from-emerald-500/8 to-transparent',
  },
]

const services = [
  { name: 'api-gateway', team: 'Platform', replicas: '3/3', status: 'Healthy', lang: 'Go' },
  { name: 'auth-service', team: 'Security', replicas: '2/2', status: 'Healthy', lang: 'C#' },
  { name: 'catalog-service', team: 'Platform', replicas: '1/3', status: 'Degraded', lang: 'C#' },
  { name: 'payment-service', team: 'Finance', replicas: '2/2', status: 'Healthy', lang: 'Java' },
  { name: 'notification-svc', team: 'Core', replicas: '1/1', status: 'Running', lang: 'Node' },
  { name: 'ml-inference', team: 'ML', replicas: '0/2', status: 'Failed', lang: 'Python' },
]

const pipelines = [
  { name: 'api-gateway', branch: 'main', time: '2m ago', status: 'Running', duration: '1m 24s' },
  { name: 'auth-service', branch: 'release/v2', time: '15m ago', status: 'Success', duration: '3m 12s' },
  { name: 'frontend', branch: 'feature/ui', time: '1h ago', status: 'Failed', duration: '45s' },
  { name: 'catalog-service', branch: 'main', time: 'queued', status: 'Pending', duration: '--' },
  { name: 'payment-svc', branch: 'hotfix/tax', time: '3h ago', status: 'Success', duration: '2m 55s' },
]

const kpis = [
  { label: 'Deployment Frequency', value: '12/day', change: '+18%', up: true, desc: 'Elite performer' },
  { label: 'Lead Time for Changes', value: '2.4h', change: '-12%', up: true, desc: 'Low category' },
  { label: 'Mean Time to Restore', value: '18min', change: '-25%', up: true, desc: 'Elite performer' },
  { label: 'Change Failure Rate', value: '2.1%', change: '-5%', up: true, desc: 'Low category' },
]

const resources = [
  { label: 'CPU Usage', value: 68, color: 'from-blue-500 to-cyan-400', warn: 80 },
  { label: 'Memory', value: 72, color: 'from-violet-500 to-purple-400', warn: 85 },
  { label: 'Storage', value: 45, color: 'from-emerald-500 to-teal-400', warn: 90 },
  { label: 'Network I/O', value: 38, color: 'from-amber-500 to-orange-400', warn: 80 },
]

const statusMap: Record<string, { color: string; bg: string; icon: React.ReactNode }> = {
  Healthy:  { color: 'text-emerald-400', bg: 'bg-emerald-400/10 border-emerald-400/20', icon: <CheckCircle2 className="w-3 h-3" /> },
  Running:  { color: 'text-blue-400',    bg: 'bg-blue-400/10 border-blue-400/20',    icon: <Loader2 className="w-3 h-3 animate-spin" /> },
  Degraded: { color: 'text-amber-400',   bg: 'bg-amber-400/10 border-amber-400/20',   icon: <AlertTriangle className="w-3 h-3" /> },
  Failed:   { color: 'text-red-400',     bg: 'bg-red-400/10 border-red-400/20',     icon: <XCircle className="w-3 h-3" /> },
  Success:  { color: 'text-emerald-400', bg: 'bg-emerald-400/10 border-emerald-400/20', icon: <CheckCircle2 className="w-3 h-3" /> },
  Pending:  { color: 'text-slate-400',   bg: 'bg-slate-400/10 border-slate-400/20',   icon: <Minus className="w-3 h-3" /> },
}

function Badge({ status }: { status: string }) {
  const s = statusMap[status] ?? statusMap.Pending
  return (
    <span className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-md text-[11px] font-medium border ${s.bg} ${s.color}`}>
      {s.icon}{status}
    </span>
  )
}

export default function DashboardPage() {
  return (
    <div className="space-y-5 max-w-[1400px]">

      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-bold text-white tracking-tight">Platform Dashboard</h1>
          <p className="text-slate-500 text-xs mt-0.5">Real-time overview · Last updated just now</p>
        </div>
        <button className="flex items-center gap-1.5 px-3 py-1.5 bg-white/5 hover:bg-white/8 border border-white/8 rounded-lg text-slate-400 hover:text-white text-xs transition-all">
          <RefreshCw className="w-3 h-3" /> Refresh
        </button>
      </div>

      {/* Stat Cards */}
      <div className="grid grid-cols-2 xl:grid-cols-3 gap-3">
        {statCards.map((c) => (
          <div
            key={c.label}
            className="relative bg-[#0d1526] border border-white/6 rounded-xl p-4 overflow-hidden group hover:border-white/12 transition-all cursor-pointer"
            style={{ boxShadow: `0 0 40px -10px ${c.glow}18` }}
          >
            <div className={`absolute inset-0 bg-gradient-to-br ${c.bg} pointer-events-none`} />
            <div className="relative flex items-start justify-between gap-3">
              <div className="min-w-0">
                <p className="text-slate-500 text-[11px] font-medium uppercase tracking-wider truncate">{c.label}</p>
                <p className="text-[28px] font-bold text-white mt-1 leading-none">{c.value}</p>
                <div className="flex items-center gap-1 mt-2">
                  {c.deltaUp === true && <TrendingUp className="w-3 h-3 text-emerald-400 flex-shrink-0" />}
                  {c.deltaUp === false && <TrendingDown className="w-3 h-3 text-red-400 flex-shrink-0" />}
                  <p className={`text-[11px] truncate ${c.deltaUp === true ? 'text-emerald-400' : c.deltaUp === false ? 'text-red-400' : 'text-slate-500'}`}>
                    {c.delta}
                  </p>
                </div>
              </div>
              <div className={`w-10 h-10 rounded-xl bg-gradient-to-br ${c.grad} flex items-center justify-center flex-shrink-0 shadow-lg`}
                style={{ boxShadow: `0 4px 15px ${c.glow}40` }}>
                <c.icon className="w-5 h-5 text-white" />
              </div>
            </div>
            <div className={`absolute bottom-0 left-0 right-0 h-[2px] bg-gradient-to-r ${c.grad} opacity-40 group-hover:opacity-70 transition-opacity`} />
          </div>
        ))}
      </div>

      {/* Middle Row */}
      <div className="grid grid-cols-1 lg:grid-cols-5 gap-3">

        {/* Service Health — 3 cols */}
        <div className="lg:col-span-3 bg-[#0d1526] border border-white/6 rounded-xl overflow-hidden">
          <div className="flex items-center justify-between px-4 py-3 border-b border-white/6">
            <div className="flex items-center gap-2">
              <Activity className="w-3.5 h-3.5 text-emerald-400" />
              <span className="text-white text-xs font-semibold">Service Health</span>
              <span className="w-1.5 h-1.5 rounded-full bg-emerald-400 animate-pulse ml-1" />
            </div>
            <button className="text-slate-500 hover:text-slate-300 text-[11px] flex items-center gap-1 transition-colors">
              View all <ArrowUpRight className="w-3 h-3" />
            </button>
          </div>
          <div className="divide-y divide-white/4">
            {services.map((s) => (
              <div key={s.name} className="flex items-center gap-3 px-4 py-2.5 hover:bg-white/3 transition-colors group">
                <div className={`w-1.5 h-1.5 rounded-full flex-shrink-0 ${
                  s.status === 'Healthy' ? 'bg-emerald-400' :
                  s.status === 'Running' ? 'bg-blue-400 animate-pulse' :
                  s.status === 'Degraded' ? 'bg-amber-400' : 'bg-red-400'
                }`} />
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <span className="text-white text-xs font-medium truncate">{s.name}</span>
                    <span className="text-slate-600 text-[10px] bg-white/4 px-1.5 py-0.5 rounded">{s.lang}</span>
                  </div>
                  <p className="text-slate-600 text-[11px] mt-0.5">{s.team} · {s.replicas} replicas</p>
                </div>
                <Badge status={s.status} />
              </div>
            ))}
          </div>
        </div>

        {/* Recent Pipelines — 2 cols */}
        <div className="lg:col-span-2 bg-[#0d1526] border border-white/6 rounded-xl overflow-hidden">
          <div className="flex items-center justify-between px-4 py-3 border-b border-white/6">
            <div className="flex items-center gap-2">
              <GitBranch className="w-3.5 h-3.5 text-violet-400" />
              <span className="text-white text-xs font-semibold">Pipelines</span>
            </div>
            <span className="text-slate-600 text-[10px]">GitHub Actions</span>
          </div>
          <div className="divide-y divide-white/4">
            {pipelines.map((p) => (
              <div key={p.name} className="flex items-center gap-3 px-4 py-2.5 hover:bg-white/3 transition-colors">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-1.5">
                    <span className="text-white text-xs font-medium truncate">{p.name}</span>
                  </div>
                  <div className="flex items-center gap-2 mt-0.5">
                    <span className="text-slate-600 text-[10px]">{p.branch}</span>
                    <span className="text-slate-700">·</span>
                    <Clock className="w-2.5 h-2.5 text-slate-600" />
                    <span className="text-slate-600 text-[10px]">{p.time}</span>
                  </div>
                </div>
                <div className="text-right flex-shrink-0">
                  <Badge status={p.status} />
                  <p className="text-slate-600 text-[10px] mt-1">{p.duration}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Bottom Row */}
      <div className="grid grid-cols-1 lg:grid-cols-5 gap-3">

        {/* DORA Metrics — 2 cols */}
        <div className="lg:col-span-2 bg-[#0d1526] border border-white/6 rounded-xl overflow-hidden">
          <div className="flex items-center gap-2 px-4 py-3 border-b border-white/6">
            <Eye className="w-3.5 h-3.5 text-cyan-400" />
            <span className="text-white text-xs font-semibold">DORA Metrics</span>
            <span className="ml-auto text-slate-600 text-[10px]">Last 30 days</span>
          </div>
          <div className="grid grid-cols-2 divide-x divide-y divide-white/4">
            {kpis.map((k) => (
              <div key={k.label} className="p-4 hover:bg-white/3 transition-colors">
                <p className="text-slate-500 text-[10px] font-medium uppercase tracking-wider leading-tight">{k.label}</p>
                <p className="text-xl font-bold text-white mt-2">{k.value}</p>
                <div className="flex items-center gap-1 mt-1">
                  {k.up ? <TrendingDown className="w-2.5 h-2.5 text-emerald-400" /> : <TrendingUp className="w-2.5 h-2.5 text-red-400" />}
                  <span className={`text-[10px] font-medium ${k.up ? 'text-emerald-400' : 'text-red-400'}`}>{k.change}</span>
                </div>
                <p className="text-slate-600 text-[10px] mt-0.5">{k.desc}</p>
              </div>
            ))}
          </div>
        </div>

        {/* Cluster Resources — 3 cols */}
        <div className="lg:col-span-3 bg-[#0d1526] border border-white/6 rounded-xl overflow-hidden">
          <div className="flex items-center justify-between px-4 py-3 border-b border-white/6">
            <div className="flex items-center gap-2">
              <Server className="w-3.5 h-3.5 text-blue-400" />
              <span className="text-white text-xs font-semibold">Cluster Resource Utilization</span>
            </div>
            <div className="flex items-center gap-1.5">
              <span className="w-1.5 h-1.5 rounded-full bg-emerald-400" />
              <span className="text-slate-500 text-[10px]">Production · eks-prod</span>
            </div>
          </div>
          <div className="p-4 space-y-4">
            {resources.map((r) => (
              <div key={r.label}>
                <div className="flex items-center justify-between mb-1.5">
                  <span className="text-slate-400 text-xs">{r.label}</span>
                  <div className="flex items-center gap-2">
                    <span className={`text-xs font-semibold ${r.value >= r.warn ? 'text-amber-400' : 'text-white'}`}>{r.value}%</span>
                    {r.value >= r.warn && <AlertTriangle className="w-3 h-3 text-amber-400" />}
                  </div>
                </div>
                <div className="h-1.5 bg-white/5 rounded-full overflow-hidden">
                  <div
                    className={`h-full bg-gradient-to-r ${r.color} rounded-full transition-all duration-700`}
                    style={{ width: `${r.value}%` }}
                  />
                </div>
              </div>
            ))}
          </div>

          {/* Node Summary */}
          <div className="grid grid-cols-3 divide-x divide-white/6 border-t border-white/6">
            {[
              { label: 'Nodes Ready', value: '12/12', ok: true },
              { label: 'Total Pods', value: '284', ok: true },
              { label: 'Pending Pods', value: '3', ok: false },
            ].map((n) => (
              <div key={n.label} className="px-4 py-3 text-center">
                <p className={`text-base font-bold ${n.ok ? 'text-white' : 'text-amber-400'}`}>{n.value}</p>
                <p className="text-slate-600 text-[10px] mt-0.5">{n.label}</p>
              </div>
            ))}
          </div>
        </div>
      </div>

    </div>
  )
}
