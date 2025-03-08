import { useEffect, useState } from "react";
import { IConfiguration } from "../models/Configuration";
import {
  callActivateConfiguration,
  callChangeConfigurationValue,
  callDeactivateConfiguration,
  callGetAllConfigurations,
} from "../services/configuration";
import Table, { ColumnsType } from "antd/es/table";
import { App, Input, Switch } from "antd";

export interface ConfigurationProps {
  configurations: IConfiguration[];
  updateConfigurations: (configurations: IConfiguration[]) => void;
}

const Configuration: React.FC<ConfigurationProps> = ({
  configurations,
  updateConfigurations,
}) => {
  const { message } = App.useApp();
  const [displayedConfigurations, setDisplayedConfigurations] =
    useState<IConfiguration[]>(configurations);
  useEffect(() => {
    setDisplayedConfigurations(configurations);
  }, [configurations]);
  useEffect(() => {
    const fetchConfigurations = async () => {
      try {
        const response = await callGetAllConfigurations();
        console.log(response);
        updateConfigurations(response);
      } catch (error) {
        console.error("Error fetching configurations:", error);
      }
    };
    fetchConfigurations();
  }, []);
  const handleChangeConfigurationStatus = async (
    key: string,
    isActive: boolean
  ) => {
    try {
      if (isActive) {
        await callDeactivateConfiguration(key);
      } else {
        await callActivateConfiguration(key);
      }
      updateConfigurations(
        configurations.map((configuration) =>
          configuration.key === key
            ? { ...configuration, isActive: !isActive }
            : configuration
        )
      );
      message.success("Cập nhật trạng thái thành công");
    } catch (error) {
      message.error("Cập nhật trạng thái thất bại");
    }
  };

  const handleChangeConfigurationValue = async (key: string, value: string) => {
    try {
      console.log(
        configurations.map((configuration) =>
          configuration.key === key
            ? { ...configuration, value: value.toString() }
            : configuration
        )
      );
      await callChangeConfigurationValue(key, JSON.stringify(value.toString()));
      updateConfigurations(
        configurations.map((configuration) =>
          configuration.key === key
            ? { ...configuration, value: value.toString() }
            : configuration
        )
      );
      message.success("Cập nhật giá trị thành công");
    } catch (error) {
      console.log(error);
      message.error("Cập nhật giá trị thất bại");
    }
  };

  const columns: ColumnsType<IConfiguration> = [
    {
      title: "ID",
      dataIndex: "id",
      key: "id",
    },
    {
      title: "Key",
      dataIndex: "key",
      key: "name",
    },
    {
      title: "Giá trị",
      key: "value",
      render: (value, record: IConfiguration) => (
        <Input
          value={record.value}
          onChange={(e) =>
            setDisplayedConfigurations((configs) =>
              configs.map((config) =>
                config.id !== record.id
                  ? config
                  : { ...config, value: e.target.value }
              )
            )
          }
          onPressEnter={() =>
            handleChangeConfigurationValue(record.key, record.value)
          }
        />
      ),
    },
    {
      title: "Trạng thái",
      key: "isActive",
      render: (_, record: IConfiguration) => (
        <Switch
          value={record.isActive}
          onChange={() =>
            handleChangeConfigurationStatus(record.key, record.isActive)
          }
        />
      ),
    },
  ];
  return <Table dataSource={displayedConfigurations} columns={columns}></Table>;
};

export default Configuration;
