import { NavLink } from 'react-router-dom'
import {
  LayoutDashboard, BookOpen, Rocket, GitBranch, Layers,
  Eye, Shield, Key, DollarSign, AlertTriangle, ClipboardList,
  Users, Server, Zap, ChevronRight
} from 'lucide-react'

const navGroups = [
  {
    label: 'PLATFORM',
    items: [
      { label: 'Dashboard', path: '/dashboard', icon: LayoutDashboard },
      { label: 'Service Catalog', path: '/catalog', icon: BookOpen },
      { label: 'Self Service', path: '/self-service', icon: Rocket },
    ]
  },
  {
    label: 'DELIVERY',
    items: [
      { label: 'GitOps', path: '/gitops', icon: GitBranch },
      { label: 'CI/CD', path: '/cicd', icon: Zap },
      { label: 'Kubernetes', path: '/kubernetes', icon: Layers },
      { label: 'Infrastructure', path: '/infrastructure', icon: Server },
    ]
  },
  {
    label: 'OBSERVABILITY',
    items: [
      { label: 'Observability', path: '/observability', icon: Eye },
      { label: 'DevSecOps', path: '/devsecops', icon: Shield },
      { label: 'Incidents', path: '/incidents', icon: AlertTriangle },
    ]
  },
  {
    label: 'MANAGEMENT',
    items: [
      { label: 'Vault / Secrets', path: '/vault', icon: Key },
      { label: 'Cost', path: '/cost', icon: DollarSign },
      { label: 'Audit', path: '/audit', icon: ClipboardList },
      { label: 'Teams', path: '/teams', icon: Users },
    ]
  },
]

export default function Sidebar() {
  return (
    <aside className="w-56 bg-[#0d1526] border-r border-white/5 flex flex-col h-full flex-shrink-0">
      {/* Logo */}
      <div className="px-4 py-4 border-b border-white/5">
        <div className="flex items-center gap-2.5">
          <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-violet-600 rounded-lg flex items-center justify-center shadow-lg shadow-blue-500/30">
            <Layers className="w-4 h-4 text-white" />
          </div>
          <div>
            <p className="text-white font-bold text-sm leading-none">Enterprise IDP</p>
            <p className="text-slate-500 text-xs mt-0.5">Platform Engineering</p>
          </div>
        </div>
      </div>

      {/* Nav */}
      <nav className="flex-1 px-3 py-3 space-y-4 overflow-y-auto">
        {navGroups.map((group) => (
          <div key={group.label}>
            <p className="text-slate-600 text-[10px] font-semibold tracking-widest px-2 mb-1.5">{group.label}</p>
            <div className="space-y-0.5">
              {group.items.map((item) => (
                <NavLink
                  key={item.path}
                  to={item.path}
                  className={({ isActive }) =>
                    `flex items-center gap-2.5 px-2.5 py-2 rounded-lg text-xs font-medium transition-all group relative ${
                      isActive
                        ? 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
                        : 'text-slate-500 hover:text-slate-200 hover:bg-white/5 border border-transparent'
                    }`
                  }
                >
                  {({ isActive }) => (
                    <>
                      {isActive && (
                        <span className="absolute left-0 top-1/2 -translate-y-1/2 w-0.5 h-4 bg-blue-500 rounded-r-full" />
                      )}
                      <item.icon className={`w-3.5 h-3.5 flex-shrink-0 ${isActive ? 'text-blue-400' : 'text-slate-600 group-hover:text-slate-400'}`} />
                      <span className="flex-1">{item.label}</span>
                      {isActive && <ChevronRight className="w-3 h-3 text-blue-500/50" />}
                    </>
                  )}
                </NavLink>
              ))}
            </div>
          </div>
        ))}
      </nav>

      {/* Footer */}
      <div className="px-4 py-3 border-t border-white/5">
        <div className="flex items-center gap-2">
          <div className="w-1.5 h-1.5 rounded-full bg-emerald-400 animate-pulse" />
          <p className="text-slate-600 text-xs">All systems operational</p>
        </div>
        <p className="text-slate-700 text-[10px] mt-1">v1.0.0 · Phase 1-3 Complete</p>
      </div>
    </aside>
  )
}
