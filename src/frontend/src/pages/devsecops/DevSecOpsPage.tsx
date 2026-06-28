import { useState } from 'react'
import { Shield, AlertTriangle, CheckCircle2, XCircle, Clock, Search, RefreshCw } from 'lucide-react'

const mockScans = [
  { id: '1', service: 'api-gateway', type: 'SAST', tool: 'SonarQube', status: 'passed', score: 92, issues: { critical: 0, high: 1, medium: 3, low: 8 }, scannedAt: '2 hours ago' },
  { id: '2', service: 'auth-service', type: 'Container', tool: 'Trivy', status: 'failed', score: 61, issues: { critical: 2, high: 5, medium: 8, low: 12 }, scannedAt: '4 hours ago' },
  { id: '3', service: 'frontend', type: 'Dependency', tool: 'OWASP', status: 'warning', score: 78, issues: { critical: 0, high: 2, medium: 6, low: 4 }, scannedAt: '1 day ago' },
  { id: '4', service: 'payment-service', type: 'SAST', tool: 'SonarQube', status: 'passed', score: 95, issues: { critical: 0, high: 0, medium: 2, low: 5 }, scannedAt: '3 hours ago' },
  { id: '5', service: 'notification-svc', type: 'Container', tool: 'Trivy', status: 'warning', score: 74, issues: { critical: 0, high: 3, medium: 5, low: 9 }, scannedAt: '6 hours ago' },
  { id: '6', service: 'user-service', type: 'Dependency', tool: 'OWASP', status: 'passed', score: 88, issues: { critical: 0, high: 1, medium: 2, low: 6 }, scannedAt: '5 hours ago' },
]

const statusColor: Record<string, string> = { passed: 'oklch(0.6 0.18 150)', failed: 'oklch(0.6 0.22 27)', warning: 'oklch(0.75 0.18 80)' }
const statusIcon: Record<string, any> = { passed: CheckCircle2, failed: XCircle, warning: AlertTriangle }
const typeColor: Record<string, string> = { SAST: 'oklch(0.6 0.2 250)', Container: 'oklch(0.6 0.2 200)', Dependency: 'oklch(0.65 0.18 80)' }

function ScoreRing({ score }: { score: number }) {
  const color = score >= 85 ? 'oklch(0.6 0.18 150)' : score >= 70 ? 'oklch(0.75 0.18 80)' : 'oklch(0.6 0.22 27)'
  return (
    <div style={{ width: 48, height: 48, borderRadius: '50%', border: '3px solid ' + color, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
      <span style={{ fontSize: 13, fontWeight: 700, color }}>{score}</span>
    </div>
  )
}

export default function DevSecOpsPage() {
  const [search, setSearch] = useState('')
  const [filter, setFilter] = useState('All')
  const [scanning, setScanning] = useState<string | null>(null)

  const filtered = mockScans.filter(s =>
    (filter === 'All' || s.status === filter.toLowerCase() || s.type === filter) &&
    (s.service.includes(search) || s.tool.toLowerCase().includes(search.toLowerCase()))
  )

  const totalCritical = mockScans.reduce((a, s) => a + s.issues.critical, 0)
  const totalHigh = mockScans.reduce((a, s) => a + s.issues.high, 0)
  const passed = mockScans.filter(s => s.status === 'passed').length
  const avgScore = Math.round(mockScans.reduce((a, s) => a + s.score, 0) / mockScans.length)

  const handleScan = async (id: string) => {
    setScanning(id)
    await new Promise(r => setTimeout(r, 2000))
    setScanning(null)
  }

  return (
    <div style={{ padding: '1.5rem', maxWidth: 1100, margin: '0 auto' }}>
      <div style={{ marginBottom: "1.5rem" }}>
        <h1 style={{ fontSize: 22, fontWeight: 700, color: 'oklch(0.985 0 0)', margin: '0 0 0.25rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
          <Shield size={22} color='oklch(0.6 0.2 250)' /> DevSecOps
        </h1>
        <p style={{ color: 'oklch(0.55 0 0)', margin: 0, fontSize: 13 }}>Security scanning — SonarQube, Trivy, OWASP</p>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '1rem', marginBottom: '1.5rem' }}>
        {[
          { label: 'Avg Security Score', value: avgScore + '/100', color: 'oklch(0.6 0.18 150)', icon: Shield },
          { label: 'Critical Issues', value: totalCritical, color: 'oklch(0.6 0.22 27)', icon: XCircle },
          { label: 'High Issues', value: totalHigh, color: 'oklch(0.75 0.18 80)', icon: AlertTriangle },
          { label: 'Scans Passed', value: passed + '/' + mockScans.length, color: 'oklch(0.6 0.18 150)', icon: CheckCircle2 },
        ].map(({ label, value, color, icon: Icon }) => (
          <div key={label} style={{ padding: '1.25rem', borderRadius: 10, background: 'oklch(0.14 0 0)', border: '1px solid oklch(0.22 0 0)' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '0.5rem' }}>
              <Icon size={14} color={color} />
              <span style={{ fontSize: 12, color: 'oklch(0.55 0 0)' }}>{label}</span>
            </div>
            <div style={{ fontSize: 24, fontWeight: 700, color }}>{value}</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'flex', gap: '0.75rem', marginBottom: '1.25rem', flexWrap: 'wrap' }}>
        <div style={{ position: 'relative', flex: 1, minWidth: 200 }}>
          <Search size={14} style={{ position: 'absolute', left: 10, top: '50%', transform: 'translateY(-50%)', color: 'oklch(0.45 0 0)' }} />
          <input value={search} onChange={e => setSearch(e.target.value)} placeholder="Search service or tool..."
            style={{ width: '100%', padding: '0.55rem 0.75rem 0.55rem 2rem', borderRadius: 6, background: 'oklch(0.14 0 0)', border: '1px solid oklch(0.25 0 0)', color: 'oklch(0.985 0 0)', fontSize: 13, boxSizing: 'border-box' }} />
        </div>
        <div style={{ display: 'flex', gap: '0.4rem', flexWrap: 'wrap' }}>
          {['All', 'passed', 'warning', 'failed', 'SAST', 'Container', 'Dependency'].map(f => (
            <button key={f} onClick={() => setFilter(f)} style={{
              padding: '0.4rem 0.85rem', borderRadius: 6, fontSize: 12, fontWeight: 500, cursor: 'pointer', border: '1px solid',
              borderColor: filter === f ? 'oklch(0.6 0.2 250)' : 'oklch(0.25 0 0)',
              background: filter === f ? 'oklch(0.6 0.2 250 / 0.15)' : 'transparent',
              color: filter === f ? 'oklch(0.6 0.2 250)' : 'oklch(0.55 0 0)',
            }}>{f}</button>
          ))}
        </div>
      </div>

      <div style={{ display: 'grid', gap: '0.75rem' }}>
        {filtered.map(scan => {
          const StatusIcon = statusIcon[scan.status]
          const sColor = statusColor[scan.status]
          const isScanning = scanning === scan.id
          return (
            <div key={scan.id} style={{ padding: '1.25rem', borderRadius: 10, background: 'oklch(0.14 0 0)', border: '1px solid oklch(0.22 0 0)', display: 'flex', alignItems: 'center', gap: '1rem' }}>
              <ScoreRing score={scan.score} />
              <div style={{ flex: 1 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', marginBottom: '0.5rem', flexWrap: 'wrap' }}>
                  <span style={{ fontWeight: 700, color: 'oklch(0.985 0 0)', fontSize: 14 }}>{scan.service}</span>
                  <span style={{ padding: '0.2rem 0.5rem', borderRadius: 4, background: typeColor[scan.type] + '20', color: typeColor[scan.type], fontSize: 11, fontWeight: 600 }}>{scan.type}</span>
                  <span style={{ padding: '0.2rem 0.5rem', borderRadius: 4, background: 'oklch(0.2 0 0)', color: 'oklch(0.6 0 0)', fontSize: 11 }}>{scan.tool}</span>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '0.3rem' }}>
                    <StatusIcon size={13} color={sColor} />
                    <span style={{ fontSize: 12, color: sColor, fontWeight: 600, textTransform: 'capitalize' }}>{scan.status}</span>
                  </div>
                </div>
                <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap' }}>
                  {[{ label: 'Critical', val: scan.issues.critical, color: 'oklch(0.6 0.22 27)' }, { label: 'High', val: scan.issues.high, color: 'oklch(0.75 0.18 80)' }, { label: 'Medium', val: scan.issues.medium, color: 'oklch(0.6 0.2 200)' }, { label: 'Low', val: scan.issues.low, color: 'oklch(0.55 0 0)' }].map(({ label, val, color }) => (
                    <span key={label} style={{ fontSize: 12, color: 'oklch(0.55 0 0)' }}><span style={{ color, fontWeight: 700 }}>{val}</span> {label}</span>
                  ))}
                  <span style={{ fontSize: 12, color: 'oklch(0.45 0 0)', display: 'flex', alignItems: 'center', gap: '0.3rem' }}>
                    <Clock size={10} /> {scan.scannedAt}
                  </span>
                </div>
              </div>
              <button onClick={() => handleScan(scan.id)} disabled={isScanning} style={{
                display: 'flex', alignItems: 'center', gap: '0.4rem', padding: '0.5rem 1rem', borderRadius: 6,
                background: isScanning ? 'oklch(0.22 0 0)' : 'oklch(0.6 0.2 250 / 0.15)',
                color: isScanning ? 'oklch(0.5 0 0)' : 'oklch(0.6 0.2 250)',
                border: '1px solid oklch(0.6 0.2 250 / 0.3)', cursor: isScanning ? 'not-allowed' : 'pointer',
                fontSize: 12, fontWeight: 600, whiteSpace: 'nowrap',
              }}>
                <RefreshCw size={12} />
                {isScanning ? 'Scanning...' : 'Re-scan'}
              </button>
            </div>
          )
        })}
      </div>
    </div>
  )
}