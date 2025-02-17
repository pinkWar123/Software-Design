import React, { useEffect, useState } from 'react';
import { Card, Typography } from 'antd';

const { Text } = Typography;

interface VersionInfo {
    version: string;
    buildTime: string;
    environment: string;
}

const VersionInfo: React.FC = () => {
    const [versionInfo, setVersionInfo] = useState<VersionInfo | null>(null);

    useEffect(() => {
        const fetchVersion = async () => {
            try {
                const response = await fetch('http://localhost:5215/api/Version');
                const data = await response.json();
                setVersionInfo(data);
            } catch (error) {
                console.error('Failed to fetch version info:', error);
            }
        };

        fetchVersion();
    }, []);

    if (!versionInfo) return null;

    return (
        <Card size="small" style={{ position: 'fixed', bottom: 20, right: 20, width: 200 }}>
            <div>
                <Text strong>Phiên bản: </Text>
                <Text>{versionInfo.version}</Text>
            </div>
            <div>
                <Text strong>Build time: </Text>
                <Text>{versionInfo.buildTime}</Text>
            </div>
            <div>
                <Text strong>Môi trường: </Text>
                <Text>{versionInfo.environment}</Text>
            </div>
        </Card>
    );
};

export default VersionInfo; 